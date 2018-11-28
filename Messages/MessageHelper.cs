using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using GDPR.Common.Core;
using System.IO;
using GDPR.Common.Encryption;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PGPSnippet.Keys;
using PGPSnippet.PGPDecryption;
using PGPSnippet.PGPEncryption;

namespace GDPR.Common.Messages
{
    public class MessageHelper
    {
        static public GDPRCore core;

        static public bool ValidateMessage(GDPRMessageWrapper message)
        {
            //download the application public key
            string applicationPublicKey = message.Source.ApiEndPoint + "/GetApplicationPublicKey?applicationid=" + message.ApplicationId;
            string systemPublicKey = message.Source.ApiEndPoint + "/GetSystemPublicKey";

            //message
            string check = Encryption.EncryptionHelper.DecryptPGP(systemPublicKey, message.Check);

            //check signer...
            bool validSigner = true;

            if (check == "GDPRISEASY")
                return true;

            return true;
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
                    MessageHelper.SendMessageViaQueue(msg, ctx);
                    break;
            }
        }

        static public string DecryptMessage(string message, EncryptionContext ctx)
        {
            Stream inputStream = Utility.GenerateStreamFromString(message);
            string passPhrase = ctx.Password;
            string privateKeyStr = EncryptionHelper.GetPrivateKey(ctx.Path, ctx.Id);
            Stream keyIn = Utility.GenerateStreamFromString(privateKeyStr);
            //PgpSecretKey keyIn = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);
            Stream outputStream = new MemoryStream();
            PGPDecrypt.Decrypt(inputStream, keyIn, passPhrase, outputStream);
            return Utility.StreamToString(outputStream);
        }

        static public string SignAndEncryptMessage(BaseGDPRMessage message, EncryptionContext ctx)
        {
            string msg = Utility.SerializeObject(message, 1);

            string publicKeyStr = EncryptionHelper.GetSystemKey();
            string privateKeyStr = EncryptionHelper.GetPrivateKey(ctx.Path, ctx.Id);

            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr);

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
            GDPRMessageWrapper w = new GDPRMessageWrapper();
            w.IsEncrypted = ctx.Encrypt;

            if (message.IsSystem)
                w.Check = EncryptionHelper.Encrypt("GDPRISEASY", ctx);
            else
                w.Check = EncryptionHelper.Encrypt("GDPRISEASY", ctx);

            w.ApplicationId = message.ApplicationId.ToString();
            //BaseProcessor p = Utility.GetProcessor<BaseProcessor>(core.GetSystemId());
            //w.Source = Utility.TrimObject<BaseProcessor>(p, 1);

            string toSend = "";

            if (ctx.Encrypt)
            {
                toSend = SignAndEncryptMessage(message, ctx);
            }

            //finish up
            w.Type = message.GetType().AssemblyQualifiedName;
            w.Object = toSend;
            w.MessageDate = DateTime.Now;
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
            client.BaseAddress = new Uri("https://admin.thegdprregistry.com");
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

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);

            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
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
            SendMessageViaQueue(inMsg, connectionStringBuilder, ctx);
        }

        static public void SendMessageViaQueue(GDPRMessageWrapper inMsg)
        {
            string hubName = Utility.GetConfigurationValue("EventHubName");
            string connectionStringBuilder = Utility.GetConfigurationValue("EventHubConnectionString") + ";EntityPath=" + hubName;
            SendMessageViaQueue(inMsg, connectionStringBuilder);
        }

    }
}
