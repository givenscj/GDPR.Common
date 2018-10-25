using System;
using System.Collections;
using System.Collections.Generic;
using GDPR.Common.Classes;
using GDPR.Common.Messages;
using GDPR.Common.Classes;
using GDPR.Common;

namespace GDPR.Common.Core
{
    public interface IGDPRCore
    {
        bool ProcessApplicationMessage(BaseApplicationMessage am);
        Guid GetSystemId();
        bool IsValidEmail(string email);
        string Encrypt(string input);
        void Log(Exception ex, string type);
        Hashtable LoadProperties();
        void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite);
        string GetConfigurationProperty(string name);
        void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records);
        void SaveApplicationRequest(Guid subjectRequestApplicationId, string v);
        string Encrypt(string v, int systemKeyVersion);
        string UploadBlob(Guid applicationId, string filePath);
        void SendMessage(BaseGDPRMessage cm);
        BaseAddress GeocodeAddress(object p, string address);
    }
}
