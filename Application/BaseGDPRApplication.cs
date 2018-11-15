using GDPR.Common;
using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Common.Data;
using GDPR.Common.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace GDPR.Applications
{
    public abstract class BaseGDPRApplication : GDPRApplicationCore, IGDPRDataSubjectActions
    {
        protected bool _supportsPersonalSearch;
        protected bool _supportsEmailSearch;
        protected bool _supportsPhoneSearch;
        protected bool _supportsAddressSearch;
        protected bool _supportsIdentitySearch;
        protected bool _supportsSocialSearch;
        protected bool _supportsIpAddressSearch;

        protected bool _enableNameSearch;
        protected bool _enablePhoneFormatsSearch;

        protected bool _supportsGDPRUpdate;
        protected bool _supportsGDPRHold;
        protected bool _supportsGDPRInsert;

        protected bool _manualApprovalOnly;
        protected bool _manualDataExportOnly;

        protected bool _supportsAnonymization;

        //allowing and implementing "name" search is in general a bad idea (if you do this, ensure that pre and post approval is enabled)
        protected string _version;
        protected IGDPRCore core;

        protected BaseApplicationMessage _request;
        protected BaseApplicationMessage _response;

        public EncryptionContext ctx { get; set; }

        public bool SupportsGDPRUpdate { get { return this._supportsGDPRUpdate; } }
        public bool SupportsGDPRHold { get { return this._supportsGDPRHold; } }
        public bool SupportsGDPRInsert { get { return this._supportsGDPRInsert; } }

        public bool SupportsEmailSearch { get { return this._supportsEmailSearch; } }
        public bool SupportsPersonalSearch { get { return this._supportsPersonalSearch; } }
        public bool SupportsPhoneSearch { get { return this._supportsPhoneSearch; } }
        public bool SupportsAddressSearch { get { return this._supportsAddressSearch; } }
        public bool SupportsIdentitySearch { get { return this._supportsIdentitySearch; } }
        public bool SupportsSocialSearch { get { return this._supportsSocialSearch; } }
        public bool SupportsIpAddressSearch { get { return this._supportsIpAddressSearch; } }

        public bool SupportsAnonymization { get { return this._supportsAnonymization; } }

        public string Version
        {
            get
            {
                return this._version;
            }
        }

        public bool ManualApprovalOnly
        {
            get
            {
                return this._manualApprovalOnly;
            }
        }

        public bool ManualDataExportOnly
        {
            get
            {
                return this._manualDataExportOnly;
            }
        }

        public BaseApplicationMessage Request
        {
            get
            {
                return this._request;
            }
            set
            {
                this.Context = value.Context;
                this._request = value;
            }
        }

        public BaseApplicationMessage Response
        {
            get
            {
                return this._response;
            }
            set
            {
                this._response = value;
            }
        }

        public BaseGDPRApplication()
        {
            _version = "1.0.0.0";
        }

        public bool SupportsOAuth { get; set; }
        public bool IsOnpremises { get; set; }
        public bool ZipAndLockExport { get; set; }
        public SecurityContext Context { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid TemplateId { get; set; }
        public Hashtable Properties { get; set; }
        public int BatchSize { get; set; }
        public string AuthCookie { get; set; }

        protected bool EnableNameSearch
        {
            get { return _enableNameSearch; }
            set { _enableNameSearch = value; }
        }

        protected bool EnablePhoneFormatsSearch
        {
            get { return _enablePhoneFormatsSearch; }
            set { _enablePhoneFormatsSearch = value; }
        }

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

        public virtual List<GDPRSubject> SubjectSearch(GDPRSubject subject)
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

            EncryptionContext ctx = new EncryptionContext();
            ctx.Encrypt = true;
            ctx.Path = Utility.GetConfigurationValue("PrivateKeyPath"); // ConfigurationManager.AppSettings["PrivateKeyPath"];
            ctx.Id = message.ApplicationId.ToString();
            ctx.Password = Utility.GetConfigurationValue("PrivateKeyPassword");

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
                        em.BlobUrl = GDPRCore.Current.Encrypt("http://empty");
                        Response = em;

                        MessageHelper.SendMessage(em, ctx);
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

                        MessageHelper.SendMessage(em, ctx);
                    }

                    return;
            }
        }

        public virtual bool RecordCreateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordDeleteIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual void RecordHold(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual void RecordNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual List<Record> GetAllRecords(GDPRSubject search)
        {
            var results = new List<Record>();
            return results;
        }

        public virtual bool RecordUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual void ValidateSubject(GDPRSubject subject)
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
            return null;
        }

        public virtual void Init()
        {
            Properties = GDPRCore.Current.LoadProperties(this.ApplicationId);
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

        override public string GetProperty(string name)
        {
            if (this.Properties == null)
                this.Properties = new Hashtable();

            string ret = null;

            BaseEntityProperty ep = (BaseEntityProperty)this.Properties[name];

            if (ep != null)
            {
                if (ep.IsSecure && ep.SystemPinVersion.HasValue)
                    ret = GDPRCore.Current.Decrypt(ep.Value, ep.SystemPinVersion.Value);
                else
                    ret = ep.Value;

                return ret;
            }

            return ret;
        }

        public override string ExportData(List<Record> records)
        {
            return base.ExportData(records);
        }

        public virtual void Discover(DateTime? checkPoint)
        {
            EncryptionContext ctx = new EncryptionContext();
            ctx.Encrypt = true;
            ctx.Path = ConfigurationManager.AppSettings["PrivateKeyPath"];
            ctx.Id = ConfigurationManager.AppSettings["ApplicationId"];
            ctx.Password = ConfigurationManager.AppSettings["PrivateKeyPassword"];

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

                MessageHelper.SendMessage(discoverMsg, ctx);
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