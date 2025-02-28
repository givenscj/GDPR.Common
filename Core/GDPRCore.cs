﻿using GDPR.Common.Classes;
using GDPR.Common.Data;
using GDPR.Common.Encryption;
using GDPR.Common.EntityProperty;
using GDPR.Common.Enums;
using GDPR.Common.Exceptions;
using GDPR.Common.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace GDPR.Common.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GDPRCore : IGDPRCore
    {
        static private IGDPRCore _current;
        /// <summary>
        /// 
        /// </summary>
        public static IGDPRCore Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="systemKeyVersion"></param>
        /// <returns></returns>
        public string Decrypt(string input, int systemKeyVersion)
        {
            return input;
        }

        public string Encrypt(string input)
        {
            return input;
        }

        public string Encrypt(string input, int systemKeyVersion)
        {
            return input;
        }

        public void ErrorSubjectRequest(Exception ex, BaseGDPRMessage baseGDPRMessage)
        {
            Log(ex.Message);
        }

        public BaseAddress GeocodeAddress(object p, string address)
        {
            BaseAddress a = new BaseAddress();
            return a;
        }

        public string GetApplicationClass(Guid applicationId)
        {
            return "";
        }

        public string GetApplicationKey(string applicationId)
        {
            return "GDPRISEASY";
        }

        public string GetApplicationKey(string applicationId, int keyVersion)
        {
            return "GDPRISEASY";
        }

        public int GetApplicationKeyVersion(Guid applicationId)
        {
            return -1;
        }

        public Guid GetApplicationTenantId(Guid applicationId)
        {
            return Guid.Empty;
        }

        public string GetConfigurationProperty(string name)
        {
            return "";
        }

        public void GetOffset(string eventHubNamespace, string hubName, string consumerGroupName, string partitionId, out DateTime checkPoint, out string offSet)
        {
            checkPoint = DateTime.Now;
            offSet = "0";
        }

        public bool SetOffSet(string eventHubNamespace, string hubName, string consumerGroup, string partitionId, DateTime lastMessageDate, string offset)
        {
            return true;
        }

        public Guid GetSystemId()
        {
            return Configuration.SystemId;
        }

        public string GetSystemKey(string systemId, int keyVersion)
        {
            return "GDPRISEASY";
        }

        public string GetSystemKey(string systemId)
        {
            return "GDPRISEASY";
        }

        public int GetSystemKeyVersion(Guid systemId)
        {
            return int.Parse(Configuration.SystemPinVersion);
        }

        public bool IsValidEmail(string email)
        {
            return true;
        }

        public Hashtable LoadProperties(Guid entityId)
        {
            return new Hashtable();
        }

        public void Log(Exception ex, string type)
        {
            return;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);

            LogToFile(message);
        }

        public void Log(Exception ex, LogLevel level)
        {
            Console.WriteLine(ex.Message);

            LogToFile(ex.Message);
            LogToFile(ex.StackTrace);
        }

        public void LogToFile(string message)
        {
            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            bool wasWritten = false;

            while (!wasWritten)
            {
                try
                {
                    File.AppendAllText($"{fi.Directory.FullName}\\Log.txt", $"[{DateTime.Now}]\t{message}\n");
                    wasWritten = true;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        public void Log(SecurityContext ctx, Exception ex, LogLevel level)
        {
            Log(ex, level);
        }

        public bool ProcessApplicationMessage(BaseApplicationMessage am)
        {
            throw new NotImplementedException();
        }

        public bool ProcessMessage(BaseGDPRMessage am)
        {
            return am.Process();
        }

        public void ProcessRequest(GDPRMessageWrapper msg)
        {
            BaseGDPRMessage actionMessage = null;
            bool success = false;

            try
            {
                EncryptionContext ctx = new EncryptionContext(msg);

                Type t = Type.GetType(msg.Type);

                if (t == null)
                    throw new GDPRException($"Type was not found {msg.Type}");

                if (!string.IsNullOrEmpty(msg.Check) && msg.IsEncrypted)
                {
                    string check = MessageHelper.DecryptMessage(msg.Check, ctx);

                    if (check != "GDPRISEASY")
                    {
                        throw new GDPRException("Encryption check failed");
                    }
                }

                if (msg.IsEncrypted)
                {
                    msg.Object = MessageHelper.DecryptMessage(msg.Object, ctx);
                    msg.IsEncrypted = false;
                }

                try
                {
                    actionMessage = (BaseGDPRMessage)Newtonsoft.Json.JsonConvert.DeserializeObject(msg.Object.ToString(), t);
                }
                catch (Exception ex)
                {
                    throw new GDPRException("Message object body not valid");
                }

                if (actionMessage == null)
                    throw new GDPRException("Message object body not valid");

                //create the system context
                if (actionMessage.Context == null)
                {
                    actionMessage.Context = new Common.Classes.SecurityContext();
                }

                actionMessage.Context.Encryption = ctx;
                actionMessage.QueueUri = msg.QueueUri;

                success = actionMessage.Process();

                GDPRCore.Current.SaveSubjectRequestMessageData(actionMessage.SubjectRequestId, JsonConvert.SerializeObject(msg));
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);

                if (ex.Message.Contains("Subject Request not found"))
                {
                    msg.IsError = true;

                    //no sense it trying it over if it was deleted...
                    msg.Retries = 3;
                }

                msg.ErrorMessage = ex.Message;

                //save the state as error...
                BaseApplicationMessage am = actionMessage as BaseApplicationMessage;

                if (am != null)
                {
                    GDPRCore.Current.SaveApplicationRequest(am, "Error");
                    GDPRCore.Current.Log(am.Context, ex, LogLevel.Error);
                }
            }

            if (!success)
            {
                GDPRCore.Current.Log("Error processing message");

                if (msg.Retries < 3)
                {
                    msg.Retries += 1;
                }
                else
                    msg.IsError = true;

                if (actionMessage != null && string.IsNullOrEmpty(actionMessage.ErrorMessage))
                {
                    msg.ErrorMessage = actionMessage.ErrorMessage;
                    msg.QueueUri = actionMessage.QueueUri;
                }

                if (msg.IsError)
                {
                    BaseErrorMessage errMsg = new BaseErrorMessage();
                    errMsg.SubjectRequestId = actionMessage.SubjectRequestId;
                    errMsg.ApplicationId = actionMessage.ApplicationId;
                    msg.Object = JsonConvert.SerializeObject(errMsg);
                    msg.QueueUri = actionMessage.QueueUri;

                    //GDPRCore.Current.SaveSubjectRequestMessageData(actionMessage.SubjectRequestId, JsonConvert.SerializeObject(msg));
                    //GDPRCore.Current.SaveApplicationRequest(actionMessage.SubjectRequestId, actionMessage.ApplicationId, "Error", msg.ErrorMessage);
                }

                //update the message date so we process it again...
                msg.MessageDate = DateTime.Now;

                GDPRCore.Current.SendMessage(msg);
            }
        }

        public void SaveApplicationRequest(Guid subjectRequestApplicationId, string v)
        {
            return;
        }

        public void SaveApplicationRequest(Guid subjectRequestId, Guid applicationId, string status, string errorMessage)
        {
            return;
        }

        public void SaveApplicationRequest(BaseApplicationMessage am, string v)
        {
            return;
        }

        public void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite)
        {
            return;
        }

        public void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records)
        {
            return;
        }

        public void SaveSubjectRequestMessageData(Guid subjectRequestId, string message)
        {
            return;
        }

        public void SendMessage(BaseGDPRMessage cm, EncryptionContext ctx)
        {
            string mode = Utility.GetConfigurationValue("Mode"); ;

            MessageHelper.SendMessage(cm, mode, ctx);
        }

        public void SendMessage(GDPRMessageWrapper msg)
        {
            MessageHelper.SendMessage(msg);            
        }

        

        public void SetSystemOAuth(OAuthContext ctx, string type)
        {
            switch (type)
            {
                case "Azure":
                    //fall back to the system application...
                    ctx.ClientId = Configuration.AzureClientId;
                    ctx.ClientSecret = Configuration.AzureClientSecret;
                    break;
                case "Dynamics":
                    //fall back to the system application...
                    ctx.ClientId = Configuration.DynamicsClientId;
                    ctx.ClientSecret = Configuration.DynamicsClientSecret;
                    break;
            }
        }

        public string UploadBlob(Guid applicationId, string filePath)
        {
            return StorageContext.Current.UploadExportBlob(applicationId, filePath);
        }

        public void UpdateApplicationStatus(Guid applicationId, string status)
        {
            return;
        }

        public string GetApplicationEventHub(string applicationId)
        {
            //get the event hub info
            HttpHelper hh = new HttpHelper();

            //get this information locally first...
            string eventHubConnectionString = Configuration.GetSetting("QueueUri");

            //if not set, try to get from remote admin site...
            if (string.IsNullOrEmpty(eventHubConnectionString))
            {
                try
                {
                    eventHubConnectionString = hh.DoGet($"{Configuration.ExternalDns}/Home/GetApplicationQueue?ApplicationId{applicationId}", "");
                }
                catch (Exception ex)
                {
                    GDPRCore.Current.Log($"{Configuration.ExternalDns} did not respond with application queue. Falling back to local configuration.");

                    eventHubConnectionString = Configuration.EventHubConnectionString;
                }
            }

            return eventHubConnectionString;
        }

        public string GetEventHubConnectionString(string eventHubName)
        {
            return Configuration.EventHubConnectionString + ";EntityPath=" + eventHubName;
        }

        public string GetSystemKey(string id, string version)
        {
            //send the message to the processor endpoint...
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Utility.GetConfigurationValue("CoreSystemUrl"));
            var result = client.GetAsync($"/Home/GetSystemPublicKey?SystemId={id}&Version={version}");
            string resultContent = result.Result.Content.ReadAsStringAsync().Result;
            return resultContent.Trim();
        }

        public string GetApplicationKey(string applicationId, string version)
        {
            //send the message to the processor endpoint...
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Utility.GetConfigurationValue("CoreSystemUrl"));
            var result = client.GetAsync($"/Home/GetApplicationPublicKey?ApplicationId={applicationId}&Version={version}");
            string resultContent = result.Result.Content.ReadAsStringAsync().Result;
            return resultContent.Trim();
        }

        public string GetSystemPin(int keyVersion)
        {
            object pwd = Configuration.GetProperty("SystemPassword");

            if (pwd != null)
                return pwd.ToString();

            return "";
        }

        public string GetApplicationPin(string applicationId, int keyVersion)
        {
            return Configuration.GetProperty("ApplicationPassword").ToString();
        }

        public List<EntityPropertyTypeBase> GetEntityPropertyDefinitions()
        {
            return new List<EntityPropertyTypeBase>();
        }

        public GDPRSubject GetSubjectWithToken(Guid userId, Guid applicationId, Guid tenantId, Guid subjectId, Guid tokenId, string url, int systemKey)
        {
            HttpHelper hh = new HttpHelper();

            //get the system public key...
            string publicKey = Configuration.GetSetting("PublicKey").Replace("<br/>","\n");
            string systemPublicKey = hh.DoGet($"{url}/Home/GetSystemPublicKey?{systemKey}", "");
            string privateKey = Configuration.GetSetting("PrivateKey").Replace("<br/>", "\n");
            string password = EncryptionHelper.Encrypt(systemPublicKey, privateKey, "GDPRROCKS", Configuration.GetSetting("ApplicationPassword"));
            string auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{applicationId}:{password}"));
            
            hh.headers.Add("Authorization", $"Basic {auth}");
            string data = hh.DoGet($"{Configuration.ExternalDns}/Home/GetSubjectWithToken?applicationId={applicationId}&subjectid={subjectId}&tokenId={tokenId}", "");
            GDPRSubject s = JsonConvert.DeserializeObject<GDPRSubject>(data);
            return s;
        }

        public string GenerateDestructionCertificate(Guid requestId, RecordCollection records)
        {
            //create a PDF or word document...

            //return the URL to that documents
            throw new NotImplementedException();
        }

        public BaseEntityProperty GetEntityPropertyType(string name, string category)
        {
            return null;
        }
    }
}
