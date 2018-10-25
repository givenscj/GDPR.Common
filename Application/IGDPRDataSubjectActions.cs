using GDPR.Common.Classes;
using System;
using System.Collections.Generic;
using GDPR.Common.Messages;

namespace GDPR.Common
{
    public interface IGDPRDataSubjectActions
    {
        void ProcessRequest(BaseApplicationMessage message);
        void ValidateSubject(GDPRSubject subject);
        bool RecordCreateIn(GDPRSubject subject);
        bool RecordCreateOut(GDPRSubject subject);
        void AnonymizeRecord(Record r);
        List<Record> GetAllRecords(GDPRSubject search);
        List<GDPRSubject> SubjectSearch(GDPRSubject search, bool allowNameSearch);
        void RecordNotify(GDPRSubject subject);
        bool RecordDeleteIn(GDPRSubject subject);
        bool RecordDeleteOut(GDPRSubject subject);
        void RecordHold(GDPRSubject subject);
        bool RecordUpdateIn(GDPRSubject subject);
        bool RecordUpdateOut(GDPRSubject subject);
        List<GDPRSubject> GetAllSubjects(int skip, int count, DateTime? changeDate);
        List<BaseGDPRMessage> GetChanges(DateTime changeDate);
        string ExportData(string applicationSubjectId);
        string ExportData(string applicationSubjectId, GDPRSubject s);
        void Discover();
        void Consent();
        void Unconsent();
    }
}
