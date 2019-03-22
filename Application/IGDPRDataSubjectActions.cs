using GDPR.Common.Classes;
using GDPR.Common.Messages;
using System;
using System.Collections.Generic;

namespace GDPR.Common
{
    public interface IGDPRDataSubjectActions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ctx"></param>
        void ProcessRequest(BaseApplicationMessage message, EncryptionContext ctx);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        void ValidateSubject(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordCreateIn(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordCreateOut(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        void AnonymizeRecord(Record r);
        void AnonymizeSubject(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        List<Record> GetAllRecords(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<GDPRSubject> SubjectSearch(GDPRSubject search);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        void RecordNotify(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordDeleteIn(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordDeleteOut(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        void RecordHold(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordUpdateIn(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool RecordUpdateOut(GDPRSubject subject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <param name="changeDate"></param>
        /// <returns></returns>
        List<GDPRSubject> GetAllSubjects(int skip, int count, DateTime? changeDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeDate"></param>
        /// <returns></returns>
        List<BaseGDPRMessage> GetChanges(DateTime changeDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        ExportInfo ExportData(List<Record> records);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationSubjectId"></param>
        /// <returns></returns>
        ExportInfo ExportData(string applicationSubjectId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationSubjectId"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        ExportInfo ExportData(string applicationSubjectId, GDPRSubject s);
        /// <summary>
        /// 
        /// </summary>
        void Discover();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationSubjectId"></param>
        void Consent(string applicationSubjectId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationSubjectId"></param>
        void Unconsent(string applicationSubjectId);
    }
}
