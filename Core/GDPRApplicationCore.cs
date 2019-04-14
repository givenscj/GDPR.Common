using GDPR.Common.Classes;
using GDPR.Common.EntityProperty;
using GDPR.Common.Enums;
using GDPR.Common.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace GDPR.Common.Core
{
    public class GDPRApplicationCore : IGDPRCore
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
            throw new NotImplementedException();
        }

        public BaseAddress GeocodeAddress(object p, string address)
        {
            BaseAddress a = new BaseAddress();
            return a;
        }

        public string GetApplicationClass(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public string GetApplicationEventHub(string applicationId)
        {
            throw new NotImplementedException();
        }

        public string GetApplicationKey(string applicationId)
        {
            return Configuration.ApplicationPassword;
        }

        public string GetApplicationKey(string applicationId, int keyVersion)
        {
            return Configuration.ApplicationPassword;
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

        
        public void GetOffset(string eventHubNamespace, string hubName, string consumerGroupName, string partitionId, out DateTime checkPoint, out string offSet)
        {
            throw new NotImplementedException();
        }

        public bool SetOffSet(string eventHubNamespace, string hubName, string consumerGroupName, string partitionId, DateTime lastMessageDate, string offset)
        {
            throw new NotImplementedException();
        }

        public Guid GetSystemId()
        {
            return Guid.Parse(ConfigurationManager.AppSettings["SystemId"].ToString());
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

        public void Log(Exception ex, LogLevel level)
        {
            //string path = Utility.GetPath();
            System.IO.File.AppendAllText(@"c:\temp\error.log", ex.Message);
        }

        public void Log(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(SecurityContext context, Exception ex, LogLevel error)
        {
            throw new NotImplementedException();
        }

        public bool ProcessApplicationMessage(BaseApplicationMessage am)
        {
            throw new NotImplementedException();
        }

        public bool ProcessMessage(BaseGDPRMessage am)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(GDPRMessageWrapper msg)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SaveSubjectRequestMessageData(Guid subjectRequestId, string message)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(BaseGDPRMessage cm, EncryptionContext ctx)
        {
            string mode = ConfigurationManager.AppSettings["Mode"];

            MessageHelper.SendMessage(cm, mode, ctx);
        }

        public void SendMessage(GDPRMessageWrapper msg)
        {
            throw new NotImplementedException();
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

        public void UpdateApplicationStatus(Guid applicationId, string v)
        {
            throw new NotImplementedException();
        }

        public string UploadBlob(Guid applicationId, string filePath)
        {
            return StorageContext.Current.UploadExportBlob(applicationId, filePath);
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
            return Configuration.GetProperty("SystemPassword").ToString();
        }

        public string GetApplicationPin(string applicationId, int keyVersion)
        {
            return Configuration.GetProperty("ApplicationPassword").ToString();
        }

        public List<EntityPropertyTypeBase> GetEntityPropertyDefinitions()
        {
            return new List<EntityPropertyTypeBase>();
        }

        public GDPRSubject GetSubjectWithToken(Guid applicationId, Guid tenantId, Guid subjectId, Guid tokenId)
        {
            //TODO - get the subject via the token...
            throw new NotImplementedException();
        }

        public string GenerateDestructionCertificate(Guid requestId, RecordCollection records)
        {
            throw new NotImplementedException();
        }
    }
}
