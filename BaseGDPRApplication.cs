using System;
using System.Collections;
using System.Collections.Generic;
using GDPR.Util;
using GDPR.Util.Classes;
using GDPR.Util.Data;
using GDPR.Util.GDPRCore;
using GDPR.Util.Messages;
using GDPR.Common.Classes;

namespace GDPR.Applications
{
    public abstract class BaseGDPRApplication : GDPRApplicationCore, IGDPRDataSubjectActions
    {
        protected bool _manualApprovalOnly;
        protected bool _manualDataExportOnly;
        protected bool _supportsAddressSearch;

        protected bool _supportsAnonymization;

        protected bool _supportsEmailSearch;
        protected bool _supportsIdentitySearch;

        //allowing and implementing "name" search is in general a bad idea (if you do this, ensure that pre and post approval is enabled)
        protected bool _supportsPersonalSearch = false;
        protected bool _supportsPhoneSearch;
        protected bool _supportsSocialSearch;
        protected string _version;
        protected IGDPRCore core;

        public BaseGDPRApplication()
        {
            _version = "1.0.0.0";
        }

        public BaseApplicationMessage Request { get; set; }
        public BaseApplicationMessage Response { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid TemplateId { get; set; }
        public Hashtable Properties { get; set; }
        public int BatchSize { get; set; }
        public string AuthCookie { get; set; }
        public bool SupportsEmailSearch => _supportsEmailSearch;

        public bool SupportsPersonalSearch => _supportsPersonalSearch;
        public bool SupportsPhoneSearch => _supportsPhoneSearch;
        public bool SupportsAddressSearch => _supportsAddressSearch;
        public bool SupportsIdentitySearch => _supportsIdentitySearch;
        public bool SupportsSocialSearch => _supportsSocialSearch;

        public bool SupportsAnonymization => _supportsAnonymization;

        public string Version => _version;

        public bool ManualApprovalOnly => _manualApprovalOnly;

        public bool ManualDataExportOnly => _manualDataExportOnly;

        public virtual void Consent()
        {
            throw new NotImplementedException();
        }

        public virtual void Unconsent()
        {
            throw new NotImplementedException();
        }

        public virtual string ExportData(string applicationSubjectId, GDPRSubject s)
        {
            throw new NotImplementedException();
        }

        public virtual string ExportData(string applicationSubjectId)
        {
            throw new NotImplementedException();
        }

        public virtual List<GDPRSubject> GetAllSubjects(int skip, int count, DateTime? changeDate)
        {
            throw new NotImplementedException();
        }

        public virtual List<BaseGDPRMessage> GetChanges(DateTime changeDate)
        {
            return new List<BaseGDPRMessage>();
        }

        public virtual List<GDPRSubject> SubjectSearch(GDPRSubject subject, bool allowNameSearch)
        {
            throw new NotImplementedException();
        }

        public virtual void AnonymizeRecord(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual void Discover()
        {
            Discover(null);
        }

        public void ProcessRequest(BaseApplicationMessage message)
        {
            Request = message;
            GDPRSubject s = null;

            if (message.Subject != null)
                s = new GDPRSubject(message.Subject);

            switch (message.GetType().Name)
            {
                case "DiscoverMessage":
                    Discover();
                    break;
                case "DeleteMessage":
                    RecordDeleteIn(message.Subject);
                    break;
                case "DataRequestMessage":
                    BaseExportMessage em = null;

                    var records = GetAllRecords(s);

                    if (records.Count == 0)
                    {
                        em = new BaseExportMessage();
                        em.ApplicationId = Request.ApplicationId;
                        em.ApplicationSubjectId = Guid.Empty.ToString(); //no id on the app side...
                        em.SubjectRequestId = Request.SubjectRequestId;
                        em.Subject = Request.Subject;
                        em.BlobUrl = core.Encrypt("http://empty");
                        Response = em;
                        SendMessage(em);
                        return;
                    }
                    else
                    {
                        var storageLocation = ExportData(records);

                        //need to save to a blog storage area if too large...
                        core.SaveRequestRecords(Request.SubjectRequestApplicationId, records);

                        //send a export message...
                        em = new BaseExportMessage();
                        em.Status = "Export Processed";
                        em.ApplicationId = Request.ApplicationId;
                        em.SubjectRequestId = Request.SubjectRequestId;
                        em.Subject = Request.Subject;
                        em.BlobUrl = core.Encrypt(storageLocation);
                        Response = em;

                        SendMessage(em);
                    }

                    return;
            }
        }

        public virtual bool RecordCreateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public bool RecordCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordDeleteIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public bool RecordDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordHold(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual List<Record> GetAllRecords(GDPRSubject search)
        {
            var results = new List<Record>();
            return results;
        }

        public bool RecordUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public bool RecordUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void ValidateSubject(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }


        public void CreateSecurityProperties(bool overwrite)
        {
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "Username", Category = "Security", Name = "Username", Value = "", IsMasked = false,
                    IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "Password", Category = "Security", Name = "Password", Value = "", IsMasked = true,
                    IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "TenantId", Category = "Security", Name = "TenantId", Value = "", IsMasked = true,
                    IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "TenantDomain", Category = "Security", Name = "TenantDomain", Value = "",
                    IsMasked = true, IsSecure = true
                }, overwrite);
        }

        public void CreateOAuthProperties(bool overwrite)
        {
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "ClientId", Name = "ClientId", Category = "Security", Type = "textbox", Value = "",
                    IsMasked = false, IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "ClientSecret", Name = "ClientSecret", Category = "Security", Type = "textbox",
                    Value = "", IsMasked = true, IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "ApiKey", Name = "ApiKey", Category = "Security", Type = "textbox", Value = "",
                    IsMasked = false, IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    DisplayName = "AccessToken", Name = "AccessToken", Category = "Security", Type = "textbox",
                    Value = "", IsMasked = true, IsSecure = true
                }, overwrite);
        }

        public virtual void ValidateRecord(Record r)
        {
            var policies = GetRecordPolicy();

            if (r == null)
                return;

            //evaluate if it can be deleted...
            foreach (var ap in policies)
            {
                if (ap.RecordType == "*")
                {
                    if (r.RecordDate < DateTime.Now.AddDays(ap.MinRecordAgeDays * -1))
                    {
                        r.CanDelete = true;
                    }
                    else
                    {
                        r.CanDelete = false;
                        r.Message = string.Format("{0} : {1} : {2}\n\r", r.Type, r.RecordDate, ap.Message);
                        return;
                    }
                }

                if (ap.RecordType == r.Type)
                {
                    if (r.RecordDate < DateTime.Now.AddDays(ap.MinRecordAgeDays * -1))
                    {
                        r.CanDelete = true;
                    }
                    else
                    {
                        r.CanDelete = false;
                        r.Message = string.Format("{0} : {1} : {2}\n\r", r.Type, r.RecordDate, ap.Message);
                        ;
                        return;
                    }
                }
            }
        }

        public virtual string Authorize()
        {
            throw new NotImplementedException();
        }

        public virtual void Init()
        {
            Properties = core.LoadProperties();
        }

        public virtual void Install(bool overwrite)
        {
            BuildProperties(overwrite);
        }

        public void AddProperty(BaseEntityProperty ep, bool overwrite)
        {
            if (Properties.ContainsKey(ep.Name))
            {
                if (overwrite)
                    Properties[ep.Name] = ep;
            }
            else
            {
                Properties.Add(ep.Name, ep);
            }
        }

        public virtual void BuildProperties(bool overwrite)
        {
            if (Properties == null)
                Properties = new Hashtable();

            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "MaxQueryRequestsPerYear", Value = "1", Type = "textbox",
                    Category = "General", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "PerQueryRequestCost", Value = "5.00", Type = "textbox",
                    Category = "General", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "BatchSize", Value = "100", Type = "textbox",
                    Category = "General", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "DaysBetweenRequests", Value = "90", Type = "textbox",
                    Category = "General", IsMasked = false, IsSecure = false
                }, overwrite);

            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "DataIsSold", Value = "true", Type = "checkbox",
                    Category = "Compliance", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "DeleteRequiresApproval", Type = "checkbox",
                    Value = "true", Category = "Compliance", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "ExportRequiresApproval", Type = "checkbox",
                    Value = "true", Category = "Compliance", IsMasked = false, IsSecure = false
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(), Name = "AllowUnverifiedData", Type = "checkbox", Value = "true",
                    Category = "Compliance", IsMasked = false, IsSecure = false
                }, overwrite);
        }

        public override void SetProperty(string name, string value)
        {
            base.SetProperty(name, value);
        }

        public override string GetProperty(string name)
        {
            return base.GetProperty(name);
        }

        public override string ExportData(List<Record> records)
        {
            return base.ExportData(records);
        }

        public virtual void Discover(DateTime? checkPoint)
        {
            try
            {
                BatchSize = int.Parse(GetProperty("BatchSize"));
            }
            catch (Exception ex)
            {
                BatchSize = 1000;
            }

            var subjects = GetAllSubjects(-1, -1, checkPoint);

            if (subjects.Count > BatchSize)
            {
                var newSubjects = new List<GDPRSubject>();

                foreach (var s in subjects)
                {
                    newSubjects.Add(s);

                    if (newSubjects.Count == BatchSize)
                    {
                        //create a new message with the batch size...
                        var discoverMsg = new BaseDiscoverResponsesMessage();
                        discoverMsg.ApplicationId = ApplicationId;
                        discoverMsg.Subjects = newSubjects;
                        discoverMsg.SystemId = core.GetSystemId();
                        discoverMsg.ProcessorId = core.GetSystemId();

                        //have to wrap this in case the message is too big and it needs to be split...
                        SendDiscoveryMessage(discoverMsg);
                    }

                    if (newSubjects.Count == 0 || newSubjects.Count == BatchSize) newSubjects = new List<GDPRSubject>();
                }
            }
            else
            {
                //create a new message with the batch size...
                BaseDiscoverResponsesMessage discoverMsg = new BaseDiscoverResponsesMessage();
                discoverMsg.ApplicationId = ApplicationId;
                discoverMsg.Subjects = subjects;
                discoverMsg.SystemId = core.GetSystemId();
                discoverMsg.ProcessorId = core.GetSystemId();
                SendMessage(discoverMsg);
            }
        }

        public abstract List<Record> GetAllRecordTypes();

        public virtual List<BaseApplicationPolicy> GetRecordPolicy()
        {
            var policies = GetPolicies();

            //no policies were set...default to 1 years on all entity types
            //NOTE:  Application can choose to set the "CanDelete" on a record which will allow the deletion to proceed despite the policy
            if (policies.Count == 0)
            {
                var types = GetAllRecordTypes();

                foreach (var t in types)
                {
                    var p = new BaseApplicationPolicy();
                    p.RecordType = t.Type;
                    p.MinRecordAgeDays = 365;
                    policies.Add(p);
                }
            }

            return policies;
        }
    }
}