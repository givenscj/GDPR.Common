using GDPR.Common.Classes;
using GDPR.Common.Enums;
using GDPR.Common.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GDPR.Common.Core
{
    public interface IGDPRCore
    {
        bool ProcessMessage(BaseGDPRMessage am);
        bool ProcessApplicationMessage(BaseApplicationMessage am);
        Guid GetSystemId();
        bool IsValidEmail(string email);
        string Encrypt(string input);
        void Log(Exception ex, LogLevel level);
        void Log(Exception ex, string level);
        void Log(SecurityContext ctx, Exception ex, LogLevel level);
        void Log(string message);
        Hashtable LoadProperties(Guid entityId);
        void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite);
        void SetSystemOAuth(OAuthContext oAuthContext, string type);
        void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records);
        string GetSystemKey(string systemId, int keyVersion);
        string GetApplicationKey(string applicationId, int keyVersion);
        Guid GetApplicationTenantId(Guid applicationId);
        void SaveApplicationRequest(Guid subjectRequestApplicationId, string v);
        void SaveSubjectRequestMessageData(Guid subjectRequestId, string message);
        string Encrypt(string v, int systemKeyVersion);
        string UploadBlob(Guid applicationId, string filePath);
        void GetOffset(string consumerGroupName, string partitionId, out DateTime checkPoint, out string offSet);
        void SendMessage(BaseGDPRMessage cm, EncryptionContext ctx);
        BaseAddress GeocodeAddress(object p, string address);
        string Decrypt(string value1, int value2);
        int GetApplicationKeyVersion(Guid applicationId);
        string GetApplicationClass(Guid applicationId);
        int GetSystemKeyVersion(Guid systemId);
        DateTime GetOffset(string hubName, string partitionId);
        bool SetOffSet(string hubName, string partitionId, DateTime lastMessageDate, string offset);
        void ProcessRequest(GDPRMessageWrapper msg);
        void SaveApplicationRequest(Guid subjectRequestId, Guid applicationId, string status, string errorMessage);
        void SaveApplicationRequest(BaseApplicationMessage am, string v);
        void SendMessage(GDPRMessageWrapper msg);
        void ErrorSubjectRequest(Exception ex, BaseGDPRMessage baseGDPRMessage);
    }
}
