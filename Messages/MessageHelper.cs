﻿using GDPR.Utililty.Messages;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Messages
{
    public class MessageHelper
    {
        static public GDPRCore core;
        static public GDPRMessageWrapper CreateWrapper(BaseGDPRMessage message)
        {
            GDPRMessageWrapper w = new GDPRMessageWrapper();
            w.ApplicationId = message.ApplicationId.ToString();
            Common.Processor p = Utility.GetProcessor<Common.Processor>(core.GetSystemId());
            w.Source = Utility.TrimObject<Common.Processor>(p, 1);

            //encrypt the message

            //sign the message


            string msg = Utility.SerializeObject(message, 1);
            w.Type = message.GetType().Name;
            w.Object = msg;
            w.MessageDate = DateTime.Now;
            return w;
        }

        static public void SendMessageViaHttp(BaseGDPRMessage message)
        {
            GDPRMessageWrapper w = CreateWrapper(message);
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
