using GDPR.Util.GDPRCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDPR.Common.Classes;
using GDPR.Util.Messages;
using System.Collections;

namespace GDPR.Common
{
    public class GDPRCore : IGDPRCore
    {
        public string Encrypt(string input)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string v, int systemKeyVersion)
        {
            throw new NotImplementedException();
        }

        public string GetConfigurationProperty(string name)
        {
            throw new NotImplementedException();
        }

        public Guid GetSystemId()
        {
            throw new NotImplementedException();
        }

        public bool IsValidEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Hashtable LoadProperties()
        {
            return new Hashtable();
        }

        public void Log(Exception ex, string type)
        {
            throw new NotImplementedException();
        }

        public bool ProcessApplicationMessage(BaseApplicationMessage am)
        {
            throw new NotImplementedException();
        }

        public void SaveApplicationRequest(Guid subjectRequestApplicationId, string v)
        {
            throw new NotImplementedException();
        }

        public void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(BaseGDPRMessage cm)
        {
            throw new NotImplementedException();
        }

        public string UploadBlob(Guid applicationId, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
