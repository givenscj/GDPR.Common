using GDPR.Common.Classes;
using GDPR.Common.Enums;
using GDPR.Common.Exceptions;
using GDPR.Common.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace GDPR.Common.Core
{
    public class GDPRCore : IGDPRCore
    {
        public static IGDPRCore Current;

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
            throw new NotImplementedException();
        }

        public string GetConfigurationProperty(string name)
        {
            return "";
        }

        public DateTime GetOffset(string hubName, string partitionId)
        {
            return DateTime.Now.AddMinutes(-60);
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
            return Configuration.SystemKeyVersion;
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

            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            File.AppendAllText($"{fi.Directory.FullName}\\Log.txt", message + "\n\r");
        }

        public void Log(Exception ex, LogLevel level)
        {
            Console.WriteLine(ex.Message);

            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            File.AppendAllText($"{fi.Directory.FullName}\\Log.txt", ex.Message + "\n\r");
            File.AppendAllText($"{fi.Directory.FullName}\\Log.txt", ex.StackTrace + "\n\r");
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
                Console.WriteLine("Error processing message");

                if (msg.Retries < 3)
                {
                    msg.Retries += 1;
                }
                else
                    msg.IsError = true;

                if (actionMessage != null && string.IsNullOrEmpty(actionMessage.ErrorMessage))
                    msg.ErrorMessage = actionMessage.ErrorMessage;

                if (msg.IsError)
                {
                    GDPRCore.Current.SaveSubjectRequestMessageData(actionMessage.SubjectRequestId, JsonConvert.SerializeObject(msg));
                    GDPRCore.Current.SaveApplicationRequest(actionMessage.SubjectRequestId, actionMessage.ApplicationId, "Error", msg.ErrorMessage);
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
            throw new NotImplementedException();
        }

        public void SaveApplicationRequest(BaseApplicationMessage am, string v)
        {
            throw new NotImplementedException();
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

        public bool SetOffSet(string hubName, string partitionId, DateTime lastMessageDate, string offset)
        {
            return true;
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
            }
        }

        public string UploadBlob(Guid applicationId, string filePath)
        {
            return StorageContext.Current.UploadExportBlob(applicationId, filePath);
        }
    }
}
