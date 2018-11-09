using GDPR.Common.Classes;
using GDPR.Common.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GDPR.Common.Core
{
    public interface IGDPRCore
    {
        bool ProcessApplicationMessage(BaseApplicationMessage am);
        Guid GetSystemId();
        bool IsValidEmail(string email);
        string Encrypt(string input);
        void Log(Exception ex, string type);
        Hashtable LoadProperties(Guid entityId);
        void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite);
        void SetSystemOAuth(OAuthContext oAuthContext, string type);
        void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records);
        void SaveApplicationRequest(Guid subjectRequestApplicationId, string v);
        string Encrypt(string v, int systemKeyVersion);
        string UploadBlob(Guid applicationId, string filePath);
        void SendMessage(BaseGDPRMessage cm, EncryptionContext ctx);
        BaseAddress GeocodeAddress(object p, string address);
        string Decrypt(string value1, int value2);
    }
}
