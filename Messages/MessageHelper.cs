﻿using GDPR.Common.Core;
using GDPR.Common.Encryption;
using GDPR.Common.Exceptions;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PGPSnippet.Keys;
using PGPSnippet.PGPDecryption;
using PGPSnippet.PGPEncryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace GDPR.Common.Messages
{
    public class MessageHelper
    {
        static public GDPRCore core;

        static public bool ValidateMessage(GDPRMessageWrapper message)
        {
            HttpHelper hh = new HttpHelper();

            //testing...
            message.Source.ApiEndPoint = Configuration.ExternalDns;

            //ignore SSL
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //download the application public key
            string applicationPublicKeyUrl = "https://" + message.Source.ApiEndPoint + "/Home/GetApplicationPublicKey?applicationid=" + message.ApplicationId + "&version=" + message.KeyVersion;
            string systemPublicKeyUrl = "https://" + message.Source.ApiEndPoint + "/Home/GetSystemPublicKey?version=" + message.KeyVersion;

            string decryptKey = "";
            
            EncryptionContext ctx = null;

            if (message.IsSystem)
            {
                decryptKey = hh.DoGet(systemPublicKeyUrl, "");
                ctx = EncryptionContext.Default;
            }
            else
            {
                decryptKey = hh.DoGet(applicationPublicKeyUrl, "");
                ctx = EncryptionContext.CreateForApplication(Guid.Parse(message.ApplicationId), message.KeyVersion);
            }
            
            //message
            string check = Encryption.EncryptionHelper.DecryptPGP(message.Check, ctx);

            //check signer...
            bool validSigner = true;

            if (check == "GDPRISEASY")
                return true;

            return false;
        }

        static public void SendMessage(BaseGDPRMessage msg, EncryptionContext ctx)
        {
            string mode = Utility.GetConfigurationValue("Mode");

            SendMessage(msg, mode, ctx);
        }

        static public void SendMessage(BaseGDPRMessage msg, string mode, EncryptionContext ctx)
        {
            //send the message...
            switch (mode.ToLower())
            {
                case "http":
                    MessageHelper.SendMessageViaHttp(msg, ctx);
                    break;
                case "queue":
                default:
                    MessageHelper.SendMessageViaQueue(msg, ctx);
                    break;
            }
        }

        static public string DecryptMessage(string message, EncryptionContext ctx)
        {
            string publicKeyStr = "";
            string privateKeyStr = "";

            if (!ctx.IsApplication)
                publicKeyStr = GDPRCore.Current.GetSystemKey(ctx.Id, ctx.Version);
            else
                publicKeyStr = GDPRCore.Current.GetApplicationKey(ctx.Id, ctx.Version);
            

            privateKeyStr = EncryptionHelper.GetPrivateKey(ctx.Path, ctx.Id, ctx.Version.ToString());

            /*
            Stream inputStream = Utility.GenerateStreamFromString(message);
            string passPhrase = ctx.Password;
            Stream keyIn = Utility.GenerateStreamFromString(privateKeyStr);
            Stream outputStream = new MemoryStream();
            PGPDecrypt.Decrypt(inputStream, null, keyIn, passPhrase, outputStream);
            */

            return EncryptionHelper.DecryptAndVerify(publicKeyStr, privateKeyStr, ctx.Password, message);

            //return Utility.StreamToString(outputStream);
        }

        static public string SignAndEncryptMessage(BaseGDPRMessage message, EncryptionContext ctx)
        {
            string msg = Utility.SerializeObject(message, 1);
            string publicKeyStr = "";

            if (ctx.IsApplication)
                publicKeyStr = GDPRCore.Current.GetApplicationKey(message.ApplicationId.ToString(), ctx.Version.ToString());
            else
                publicKeyStr = GDPRCore.Current.GetSystemKey(message.SystemId.ToString(), ctx.Version.ToString());

            string privateKeyStr = EncryptionHelper.GetPrivateKey(ctx.Path, ctx.Id, ctx.Version.ToString());

            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr, false);

            string passPhrase = ctx.Password;

            PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(publicKey, secretKey, passPhrase);
            PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);

            Stream inputData = Utility.GenerateStreamFromString(msg);
            Stream encryptedMessageStream = new MemoryStream();

            encrypter.SignAndEncryptStream(inputData, encryptedMessageStream, passPhrase.ToCharArray(), true, true, publicKey, secretKey);
            string encryptedMessage = Utility.StreamToString(encryptedMessageStream);
            return encryptedMessage;
        }

        static public GDPRMessageWrapper CreateWrapper(BaseGDPRMessage message, EncryptionContext ctx)
        {
            ctx.Encrypt = Configuration.EnableEncryption;

            GDPRMessageWrapper w = new GDPRMessageWrapper
            {
                IsEncrypted = ctx.Encrypt,
                IsSystem = message.IsSystem,
                KeyVersion = ctx.Version,
                SystemId = message.SystemId.ToString()
            };

            if (ctx.Encrypt)
            {
                w.Check = EncryptionHelper.Encrypt("GDPRISEASY", ctx);
            }

            w.ApplicationId = message.ApplicationId.ToString();
            //BaseProcessor p = Utility.GetProcessor<BaseProcessor>(core.GetSystemId());
            //w.Source = Utility.TrimObject<BaseProcessor>(p, 1);

            //this should not go...
            message.Database = null;

            //send the message
            string toSend = JsonConvert.SerializeObject(message);

            if (ctx.Encrypt)
            {
                toSend = SignAndEncryptMessage(message, ctx);
            }

            //finish up
            w.Type = message.GetType().AssemblyQualifiedName;
            w.Object = toSend;
            w.QueueUri = message.QueueUri;
            w.MessageDate = DateTime.Now.AddMinutes(5);

            //throw message if message is not encrypted...
            if (!ctx.Encrypt)
            {
                GDPRCore.Current.Log(new GDPRException("Message is not encrypted"), Enums.LogLevel.Error);
            }
            return w;
        }

        private static string GetApplicationKey(GDPRMessageWrapper message)
        {
            string publicKey = message.Source.ApiEndPoint + "/GetPublicKey?applicationid=" + message.ApplicationId;
            return publicKey;
        }

        static public void SendMessageViaHttp(BaseGDPRMessage message, EncryptionContext ctx)
        {
            GDPRMessageWrapper w = CreateWrapper(message, ctx);
            SendMessageViaHttp(w);
        }

        static public void SendMessageViaHttp(GDPRMessageWrapper w)
        {
            string msgWrapper = Common.Utility.SerializeObject(w, 1);

            string post = "=" + msgWrapper.ToString();

            //send the message to the processor endpoint...
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(GDPR.Common.Configuration.ExternalDns);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("", post)
            });

            var result = client.PostAsync("/Admin/Request", content);
            string resultContent = result.Result.Content.ReadAsStringAsync().Result;

            switch (resultContent)
            {
                case "success":
                    break;
                case "failure":
                    break;
            }
        }

        static public void SendMessageViaQueue(BaseGDPRMessage inMsg, string connectionString, EncryptionContext ctx)
        {
            GDPRMessageWrapper message = CreateWrapper(inMsg, ctx);

            SendMessage(message, connectionString);
        }

        static public bool SendMessage(GDPRMessageWrapper message, string connectionString)
        {
            //set the ipaddress for tenant and application logging (used for firewall rule evaluation reporting)
            message.IpAddress = Utility.GetInternetIp()[0];

            GDPRCore.Current.Log($"Sending message wrapper : {message.Type}");

            string eventHubName = Configuration.EventHubName;

            //these need to be seperated out to keep system messages fast...
            if (message.Type.Contains("DiscoverResponseMessage"))
                eventHubName = Configuration.EventDiscoveryHubName;

            //these go directly to the applications queue...
            if (!message.IsSystem && message.Object.Contains("\"Type\":\"DiscoverMessage\""))
            {
                connectionString = GDPRCore.Current.GetApplicationEventHub(message.ApplicationId);
            }

            if (message.IsError)
                eventHubName = Configuration.EventErrorHubName;

            string connectionStringBuilder = null;

            //only do this if we didn't have something coming in...
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionStringBuilder = GDPRCore.Current.GetEventHubConnectionString(eventHubName);
                //Configuration.EventHubConnectionString + ";EntityPath=" + eventHubName;

                if (!message.IsSystem)
                {
                    connectionStringBuilder = GDPRCore.Current.GetApplicationEventHub(message.ApplicationId);

                    if (!connectionStringBuilder.Contains("EntityPath"))
                        connectionStringBuilder += $";EntityPath={message.ApplicationId}";
                }
            }
            else
                connectionStringBuilder = connectionString;

            if (!connectionStringBuilder.Contains("EntityPath"))
                connectionStringBuilder += $";EntityPath={Configuration.EventHubName}";

            //pick a different queue based on the message type..
            switch (message.Type)
            {
                case "DiscoverResponsesMessage":
                    break;
                case "DiscoverMessage":
                    break;
                case "PreApprovalMessage":
                    break;
                case "PostApprovalMessage":
                    break;
            }

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder);
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            try
            {
                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            }
            catch (Exception ex)
            {
                throw;
            }

            //eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
            //eventHubClient.CloseAsync();

            return true;
        }

        static public bool SendMessage(GDPRMessageWrapper message)
        {
            string connString = Configuration.EventHubConnectionString;

            if (!string.IsNullOrEmpty(message.QueueUri) && !message.IsError)
                connString = message.QueueUri;

            SendMessage(message, connString);

            return true;
        }

        /*
        static public bool SendMessage(GDPRMessageWrapper message)
        {
            return true;
        }
        */

        static public void SendMessageViaQueueMSI(GDPRMessageWrapper message, string eventHubName)
        {
            MessagingFactorySettings messagingFactorySettings = new MessagingFactorySettings
            {
                //TokenProvider = TokenProvider.CreateManagedServiceIdentityTokenProvider(new Uri("https://servicesbus.microsoft.net")),
                TokenProvider = TokenProvider.CreateManagedServiceIdentityTokenProvider(ServiceAudience.EventHubsAudience),
                TransportType = TransportType.Amqp
            };

            MessagingFactory messagingFactory = MessagingFactory.Create($"sb://{Configuration.ResourcePrefix + "ns-" + GDPRCore.Current.GetSystemId()}.servicebus.windows.net/",
                messagingFactorySettings);

            EventHubClient ehClient = messagingFactory.CreateEventHubClient(eventHubName);
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            ehClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            ehClient.Close();
            messagingFactory.Close();
        }

        static public void SendMessageViaQueue(GDPRMessageWrapper message, string connectionString)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
        }

        static public void SendMessageViaQueue(BaseGDPRMessage inMsg, EncryptionContext ctx)
        {
            string hubName = Utility.GetConfigurationValue("EventHubName");
            string connectionStringBuilder = Utility.GetConfigurationValue("EventHubConnectionString") + ";EntityPath=" + hubName;

            if (!string.IsNullOrEmpty(inMsg.QueueUri))
                connectionStringBuilder = inMsg.QueueUri;

            SendMessageViaQueue(inMsg, connectionStringBuilder, ctx);
        }

        static public void SendMessageViaQueue(GDPRMessageWrapper inMsg)
        {
            string hubName = Utility.GetConfigurationValue("EventHubName");

            string connectionStringBuilder = Utility.GetConfigurationValue("EventHubConnectionString") + ";EntityPath=" + hubName;

            if (Configuration.IsManaged && !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
                SendMessageViaQueueMSI(inMsg, hubName);
            }
            else
                SendMessageViaQueue(inMsg, connectionStringBuilder);
        }

        
    }
}
