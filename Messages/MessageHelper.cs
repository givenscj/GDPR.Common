using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using GDPR.Common.Core;
using System.IO;

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

        static public void SendMessage(BaseGDPRMessage msg)
        {
            string mode = ConfigurationManager.AppSettings["Mode"];
            SendMessage(msg, mode);
        }

        static public void SendMessage(BaseGDPRMessage msg, string mode)
        {
            //send the message...
            switch (mode)
            {
                case "Http":
                    MessageHelper.SendMessageViaHttp(msg);
                    break;
                case "Queue":
                    MessageHelper.SendMessageViaQueue(msg);
                    break;
            }
        }

        static public GDPRMessageWrapper CreateWrapper(BaseGDPRMessage message, bool encrypt)
        {
            GDPRMessageWrapper w = new GDPRMessageWrapper();
            w.ApplicationId = message.ApplicationId.ToString();
            BaseProcessor p = Utility.GetProcessor<BaseProcessor>(core.GetSystemId());
            w.Source = Utility.TrimObject<BaseProcessor>(p, 1);

            string toSend = "";

            string msg = Utility.SerializeObject(message, 1);
            toSend = msg;

            if (encrypt)
            {
                //encrypt the message
                string encKey = GetSystemKey();
                string enc = Encryption.EncryptionHelper.EncryptPGP3(encKey, msg);

                //sign the message
                string signingKey = GetPrivateKey(ConfigurationManager.AppSettings["PrivateKey"]);
                string signed = Encryption.EncryptionHelper.EncryptPGP3(signingKey, enc);

                toSend = signed;
            }

            //finish up
            w.Type = message.GetType().Name;
            w.Object = toSend;
            w.MessageDate = DateTime.Now;
            return w;
        }

        private static string GetSystemKey()
        {
            //send the message to the processor endpoint...
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SystemUrl"]);
            var result = client.GetAsync("/GetSystemKey");
            string resultContent = result.Result.Content.ReadAsStringAsync().Result;
            return resultContent;
        }

        private static string GetPrivateKey(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        private static string GetApplicationKey(GDPRMessageWrapper message)
        {
            string publicKey = message.Source.ApiEndPoint + "/GetPublicKey?applicationid=" + message.ApplicationId;
            return publicKey;
        }

        static public void SendMessageViaHttp(BaseGDPRMessage message)
        {
            GDPRMessageWrapper w = CreateWrapper(message, true);
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

            switch(resultContent)
            {
                case "success":
                    break;
                case "failure":
                    break;
            }
        }

        static public void SendMessageViaQueue(BaseGDPRMessage message)
        {
            string connectionStringBuilder = ConfigurationManager.AppSettings["EventHubConnectionString"] + ";EntityPath=" + ConfigurationManager.AppSettings["EventHubName"];

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
        }
    }
}
