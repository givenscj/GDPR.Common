using GDPR.Common.Classes;
using GDPR.Common.Messages;
using GDPR.Common.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

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

        public BaseAddress GeocodeAddress(object p, string address)
        {
            BaseAddress a = new BaseAddress();
            return a;
        }

        public string GetConfigurationProperty(string name)
        {
            return "";
        }

        public Guid GetSystemId()
        {
            return Guid.Parse(ConfigurationManager.AppSettings["SystemId"].ToString());
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

        public bool ProcessApplicationMessage(BaseApplicationMessage am)
        {
            throw new NotImplementedException();
        }

        public void SaveApplicationRequest(Guid subjectRequestApplicationId, string v)
        {
            return;
        }

        public void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite)
        {
            return;
        }

        public void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(BaseGDPRMessage cm)
        {
            string mode = ConfigurationManager.AppSettings["Mode"];
            MessageHelper.SendMessage(cm, mode);
        }

        public string UploadBlob(Guid applicationId, string filePath)
        {
            AzureStorage s = new AzureStorage();
            return s.UploadBlob(applicationId, filePath);
        }
    }
}
