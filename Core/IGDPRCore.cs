using GDPR.Common.Classes;
using GDPR.Common.EntityProperty;
using GDPR.Common.Enums;
using GDPR.Common.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GDPR.Common.Core
{
    public interface IGDPRCore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="am"></param>
        /// <returns></returns>
        /// <example></example>
        bool ProcessMessage(BaseGDPRMessage am);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="am"></param>
        /// <returns></returns>
        bool ProcessApplicationMessage(BaseApplicationMessage am);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Guid GetSystemId();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IsValidEmail(string email);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Encrypt(string input);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="level"></param>
        void Log(Exception ex, LogLevel level);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="level"></param>
        void Log(Exception ex, string level);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ex"></param>
        /// <param name="level"></param>
        void Log(SecurityContext ctx, Exception ex, LogLevel level);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Hashtable LoadProperties(Guid entityId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="properties"></param>
        /// <param name="overwrite"></param>
        void SaveEntityProperties(Guid applicationId, Hashtable properties, bool overwrite);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyVersion"></param>
        /// <returns></returns>
        string GetSystemPin(int keyVersion);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oAuthContext"></param>
        /// <param name="type"></param>
        void SetSystemOAuth(OAuthContext oAuthContext, string type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectRequestApplicationId"></param>
        /// <param name="records"></param>
        void SaveRequestRecords(Guid subjectRequestApplicationId, List<Record> records);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="keyVersion"></param>
        /// <returns></returns>
        string GetApplicationPin(string applicationId, int keyVersion);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="keyVersion"></param>
        /// <returns></returns>
        string GetSystemKey(string systemId, int keyVersion);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="keyVersion"></param>
        /// <returns></returns>
        string GetApplicationKey(string applicationId, int keyVersion);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Guid GetApplicationTenantId(Guid applicationId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectRequestApplicationId"></param>
        /// <param name="v"></param>
        void SaveApplicationRequest(Guid subjectRequestApplicationId, string v);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectRequestId"></param>
        /// <param name="message"></param>
        void SaveSubjectRequestMessageData(Guid subjectRequestId, string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="systemKeyVersion"></param>
        /// <returns></returns>
        string Encrypt(string v, int systemKeyVersion);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string UploadBlob(Guid applicationId, string filePath);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubNamespace"></param>
        /// <param name="hubName"></param>
        /// <param name="consumerGroupName"></param>
        /// <param name="partitionId"></param>
        /// <param name="checkPoint"></param>
        /// <param name="offSet"></param>
        void GetOffset(string eventHubNamespace, string hubName, string consumerGroupName, string partitionId, out DateTime checkPoint, out string offSet);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="ctx"></param>
        void SendMessage(BaseGDPRMessage cm, EncryptionContext ctx);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        BaseAddress GeocodeAddress(object p, string address);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        string Decrypt(string value1, int value2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        int GetApplicationKeyVersion(Guid applicationId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        string GetApplicationClass(Guid applicationId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        int GetSystemKeyVersion(Guid systemId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubNamespace"></param>
        /// <param name="hubName"></param>
        /// <param name="consumerGroupName"></param>
        /// <param name="partitionId"></param>
        /// <param name="lastMessageDate"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        bool SetOffSet(string eventHubNamespace, string hubName, string consumerGroupName, string partitionId, DateTime lastMessageDate, string offset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void ProcessRequest(GDPRMessageWrapper msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectRequestId"></param>
        /// <param name="applicationId"></param>
        /// <param name="status"></param>
        /// <param name="errorMessage"></param>
        void SaveApplicationRequest(Guid subjectRequestId, Guid applicationId, string status, string errorMessage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="am"></param>
        /// <param name="v"></param>
        void SaveApplicationRequest(BaseApplicationMessage am, string v);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void SendMessage(GDPRMessageWrapper msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="baseGDPRMessage"></param>
        void ErrorSubjectRequest(Exception ex, BaseGDPRMessage baseGDPRMessage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="v"></param>
        void UpdateApplicationStatus(Guid applicationId, string v);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        string GetApplicationEventHub(string applicationId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubName"></param>
        /// <returns></returns>
        string GetEventHubConnectionString(string eventHubName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        string GetSystemKey(string id, string version);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        string GetApplicationKey(string id, string version);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<EntityPropertyTypeBase> GetEntityPropertyDefinitions();
        GDPRSubject GetSubjectWithToken(Guid applicationId, Guid tenantId, Guid subjectId, Guid tokenId);
    }
}
