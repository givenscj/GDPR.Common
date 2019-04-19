using GDPR.Common;
using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Common.Data;
using GDPR.Common.EntityProperty;
using GDPR.Common.Exceptions;
using GDPR.Common.Messages;
using GDPR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Configuration = GDPR.Common.Configuration;

namespace GDPR.Applications
{
    public abstract class GDPRApplicationBase : GDPRApplicationCore, IGDPRDataSubjectActions
    {
        //application search support 
        protected bool _supportsPersonalSearch;
        protected bool _supportsEmailSearch;
        protected bool _supportsPhoneSearch;
        protected bool _supportsAddressSearch;
        protected bool _supportsIdentitySearch;
        protected bool _supportsSocialSearch;
        protected bool _supportsIpAddressSearch;
        protected bool _supportsBioidentitySearch;
        protected bool _supportsDeviceSearch;
        protected bool _supportsDnaSearch;

        //misc search 
        //allowing and implementing "name" search is in general a bad idea (if you do this, ensure that pre and post approval is enabled)
        protected bool _enableNameSearch;
        protected bool _enablePhoneFormatsSearch;

        //GDPR
        protected bool _supportsGDPRQuery;
        protected bool _supportsGDPRDelete;
        protected bool _supportsGDPRUpdate;
        protected bool _supportsGDPRHold;
        protected bool _supportsGDPRInsert;

        //records
        protected bool _supportsRecords;
        protected bool _supportsAnonymization;
        protected bool _allowUnverifiedRecords;

        //Approval
        protected bool _manualApprovalOnly;
        protected bool _manualDataExportOnly;

        //webhook support
        protected bool _supportsWebHookCreate;
        protected bool _supportsWebHookDelete;
        protected bool _supportsWebHookUpdate;

        //basic properties
        protected string _version;
        protected string _shortName;
        protected string _longName;
        protected string _link;
        protected IGDPRCore core;

        //in and out message for higher level processing
        protected BaseApplicationMessage _request;
        protected BaseApplicationMessage _response;

        //how to encrypt the messasges that are outgoing
        public EncryptionContext ctx { get; set; }

        public bool AllowUnverifiedRecords { get { return this._allowUnverifiedRecords; } set { this._allowUnverifiedRecords = value; } }

        public bool SupportsGDPRUpdate { get { return this._supportsGDPRUpdate; } }
        public bool SupportsGDPRHold { get { return this._supportsGDPRHold; } }
        public bool SupportsGDPRInsert { get { return this._supportsGDPRInsert; } }

        public bool SupportsEmailSearch { get { return this._supportsEmailSearch; } }
        public bool SupportsPersonalSearch { get { return this._supportsPersonalSearch; } }
        public bool SupportsPhoneSearch { get { return this._supportsPhoneSearch; } }
        public bool SupportsAddressSearch { get { return this._supportsAddressSearch; } }
        public bool SupportsIdentitySearch { get { return this._supportsIdentitySearch; } }
        public bool SupportsBioidentitySearch { get { return this._supportsBioidentitySearch; } }
        public bool SupportsDeviceSearch { get { return this._supportsDeviceSearch; } }

        public bool SupportsDnaSearch { get { return this._supportsDnaSearch; } }
        public bool SupportsSocialSearch { get { return this._supportsSocialSearch; } }
        public bool SupportsIpAddressSearch { get { return this._supportsIpAddressSearch; } }

        public bool SupportsAnonymization { get { return this._supportsAnonymization; } }

        public bool SupportsRecords { get { return this._supportsRecords; } }

        public int Tier { get; set; }

        public bool ZipAndLock { get; set; }

        public string Version
        {
            get
            {
                return this._version;
            }
        }

        public string Link
        {
            get
            {
                return this._link;
            }
        }

        public string ShortName
        {
            get
            {
                return this._shortName;
            }
        }

        public string LongName
        {
            get
            {
                return this._longName;
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

        public GDPRApplicationBase()
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

        public virtual void Consent(string applicationSubjectId)
        {
            throw new NotImplementedException();
        }

        public virtual void Unconsent(string applicationSubjectId)
        {
            throw new NotImplementedException();
        }

        public virtual ExportInfo ExportData(string applicationSubjectId, GDPRSubject s)
        {
            throw new GDPRException("ExportData[string,GDPRSubject] is not implemented.  Check the SupportsRecords flag on the applicaiton");
        }

        public virtual ExportInfo ExportData(string applicationSubjectId)
        {
            throw new GDPRException("ExportData[string]  is not implemented.  Check the SupportsRecords flag on the applicaiton");
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

        public virtual void PhoneNormalization()
        {
            throw new NotImplementedException();
        }

        public virtual void ProcessRequest(BaseGDPRMessage message, EncryptionContext ctx)
        {
            message.Process();
        }

        public virtual void ProcessRequest(BaseApplicationMessage message, EncryptionContext ctx)
        {
            Request = message;
            GDPRSubject s = null;

            if (ctx == null)
            {
                ctx = EncryptionContext.CreateForApplication(message.ApplicationId, message.Version);
            }

            if (message.Subject != null)
                s = new GDPRSubject(message.Subject);

            string action = message.GetType().Name;

            if (action == "BaseApplicationMessage")
                action = message.Type;

            switch (action)
            {
                case "DiscoverMessage":
                    Discover();
                    break;
                case "DeleteMessage":
                    
                    //delete based on the records user submitted...
                    if (message.Records != null && message.Records.Records.Count > 0)
                    {
                        foreach(Record r in message.Records.Records)
                        {
                            RecordDelete(r);
                        }
                    }
                    else
                        SubjectDeleteIn(message.Subject);

                    //create a destruction certification
                    string url = GDPRCore.Current.GenerateDestructionCertificate(this.Request.SubjectRequestApplicationId, message.Records);

                    BaseDeleteMessage msgD = new BaseDeleteMessage();
                    msgD.Status = "Delete Processed";
                    msgD.ApplicationId = Request.ApplicationId;
                    msgD.ApplicationSubjectId = Guid.Empty.ToString(); //no id on the app side...
                    msgD.SubjectRequestId = Request.SubjectRequestId;
                    msgD.Subject = Request.Subject;
                    msgD.ProcessorId = Request.ProcessorId;
                    msgD.SystemId = Request.SystemId;
                    Response = msgD;

                    DeleteInfo di = new DeleteInfo();
                    di.Urls.Add(url);
                    msgD.Info = di;

                    MessageHelper.SendMessage(msgD, ctx);

                    break;
                case "UpdateMessage":
                    SubjectUpdateIn(message.Subject);
                    break;
                case "HoldMessage":
                    SubjectHoldIn(message.Subject);
                    break;
                case "DataRequestMessage":
                    BaseExportMessage em = null;

                    var records = GetAllRecords(s);

                    //need to save to a blog storage area if too large...
                    //GDPRCore.Current.SaveRequestRecords(Request.SubjectRequestApplicationId, records);

                    em = new BaseExportMessage();

                    if (records.Count == 0)
                    {
                        em.ApplicationSubjectId = Guid.Empty.ToString(); //no id on the app side...

                        ExportInfo ei = new ExportInfo();
                        ei.Urls.Add("http://empty");
                        em.Info = ei;
                    }
                    else
                    {
                        ExportInfo storageLocation = ExportData(records);
                        em.Info = storageLocation;
                    }

                    em.Status = "Export Processed";
                    em.ApplicationId = Request.ApplicationId;
                    em.ApplicationSubjectId = Guid.Empty.ToString(); //no id on the app side...
                    em.SubjectRequestId = Request.SubjectRequestId;
                    em.Subject = Request.Subject;
                    em.ProcessorId = Request.ProcessorId;
                    em.SystemId = Request.SystemId;
                    Response = em;

                    MessageHelper.SendMessage(em, ctx);
                    return;
            }
        }

        public virtual bool TestApi()
        {
            throw new NotImplementedException();
        }

        public virtual ApplicationStatusModel CheckStatus()
        {
            ApplicationStatusModel asm = new ApplicationStatusModel();
            asm.ApplicationId = this.ApplicationId;
            asm.Status = "CheckStatus not implemented";
            return asm;            
        }

        public virtual void SubjectNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual List<Record> GetAllRecords(GDPRSubject search)
        {
            var results = new List<Record>();
            return results;
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
                    EntityId = this.ApplicationId,
                    DisplayName = "Username", Category = "Security",
                    Type = "textbox", Name = "Username", Value = "", IsMasked = false,
                    IsSecure = true
                }, overwrite);
            AddProperty(
                new BaseEntityProperty
                {
                    EntityId = this.ApplicationId,
                    DisplayName = "Password", Category = "Security",
                    Type = "textbox", Name = "Password", Value = "", IsMasked = true,
                    IsSecure = true
                }, overwrite);
        }

        public void CreateRequestProperties(bool overwrite)
        {
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsAddressSearch",
                     Category = "Supported Data",
                     Name = "SupportsAddressSearch",
                     Value = "",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsBioidentitySearch",
                     Category = "Supported Data",
                     Name = "SupportsBioidentitySearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsDeviceSearch",
                     Category = "Supported Data",
                     Name = "SupportsDeviceSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsDnaSearch",
                     Category = "Supported Data",
                     Name = "SupportsDnaSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsEmailSearch",
                     Category = "Supported Data",
                     Name = "SupportsEmailSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsIdentitySearch",
                     Category = "Supported Data",
                     Name = "SupportsIdentitySearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsIpAddressSearch",
                     Category = "Supported Data",
                     Name = "SupportsIpAddressSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsPersonalSearch",
                     Category = "Supported Data",
                     Name = "SupportsPersonalSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsPhoneSearch",
                     Category = "Supported Data",
                     Name = "SupportsPhoneSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "SupportsSocialSearch",
                     Category = "Supported Data",
                     Name = "SupportsSocialSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
            AddProperty(
                 new BaseEntityProperty
                 {
                     EntityId = this.ApplicationId,
                     DisplayName = "EnablePhoneFormatsSearch",
                     Category = "Search",
                     Name = "EnablePhoneFormatsSearch",
                     Value = "false",
                     Type = "checkbox",
                     IsMasked = false,
                     IsSecure = false
                 }, overwrite);
        }

        public void CreateOAuthProperties(bool overwrite)
        {
            AddProperty(new BaseEntityProperty() { EntityId = this.ApplicationId, EntityPropertyId = Guid.NewGuid(), DisplayName = "ClientId", Name = "ClientId", Category = "Security", Type = "textbox", Value = "",IsMasked = false, IsSecure = true}, overwrite);
            AddProperty(new BaseEntityProperty() {EntityId = this.ApplicationId, EntityPropertyId = Guid.NewGuid(),
                    DisplayName = "Client Secret", Name = "ClientSecret", Category = "Security", Type = "textbox",
                    Value = "", IsMasked = true, IsSecure = true
                }, overwrite);
            AddProperty(new BaseEntityProperty()
                {
                    EntityId = this.ApplicationId,
                    EntityPropertyId = Guid.NewGuid(),
                    DisplayName = "Api Key", Name = "ApiKey", Category = "Security", Type = "textbox", Value = "",
                    IsMasked = false, IsSecure = true
                }, overwrite);
            AddProperty(new BaseEntityProperty()
                {
                    EntityId = this.ApplicationId,
                    EntityPropertyId = Guid.NewGuid(),
                    DisplayName = "Access Token", Name = "AccessToken", Category = "Security", Type = "textbox",
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

        public Guid GetEntityPropertyTypeId(string name, string category)
        {
            List<EntityPropertyTypeBase> types = this.GetEntityPropertyDefinitions();
            types.AddRange(GDPRCore.Current.GetEntityPropertyDefinitions());

            EntityPropertyTypeBase b = types.Find(e => e.Name == name && e.Category == category);

            if (b != null)
                return b.EntityPropertyTypeId;

            return Guid.Empty;
        }

        public void AddProperty(BaseEntityProperty ep, bool overwrite)
        {
            BaseEntityProperty ept = GDPRCore.Current.GetEntityPropertyType(ep.Name, ep.Category);

            if (ept != null)
            {
                ep.EntityPropertyTypeId = ept.EntityPropertyTypeId;
                ep.Options = ept.Options;
            }

            if (string.IsNullOrEmpty(ep.DisplayName))
                ep.DisplayName = ep.Name;

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
                    EntityPropertyId = Guid.NewGuid(),
                    Name = "Department",
                    Value = "",
                    Type = "textbox",
                    Category = "Billing",
                    IsMasked = false,
                    IsSecure = false
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

            AddProperty(
                new BaseEntityProperty
                {
                    EntityPropertyId = Guid.NewGuid(),
                    Name = "RemoveMinors",
                    Type = "checkbox",
                    Value = "true",
                    Category = "Configuration",
                    IsMasked = false,
                    IsSecure = false
                }, overwrite);
        }

        public override void SetProperty(string name, string value, bool isHidden, bool persist)
        {
            base.SetProperty(name, value, isHidden, persist);
        }

        public string GetProperty(string name, string defaultValue)
        {
            string val = GetProperty(name);

            if (string.IsNullOrEmpty(val))
                return defaultValue;

            return val;
        }

        override public string GetProperty(string name)
        {
            if (this.Properties == null)
                this.Properties = new Hashtable();

            string ret = null;

            BaseEntityProperty ep = (BaseEntityProperty)this.Properties[name];

            if (ep != null)
            {
                if (ep.IsSecure && ep.IsEncrypted)
                    ret = GDPRCore.Current.Decrypt(ep.Value, ep.SystemPinVersion);
                else
                    ret = ep.Value;

                return ret;
            }

            return ret;
        }

        public override ExportInfo ExportData(List<Record> records)
        {
            if (ZipAndLock)
            {
                string code = Utility.GenerateCode();
                return ExportData(records, true, code);
            }
            else
                return ExportData(records, false, null);
        }

        public virtual ExportInfo ExportData(List<Record> records, bool zipAndLock, string password)
        {
            ExportInfo ei = new ExportInfo();

            string json = Common.Utility.SerializeObject(records, 1);

            //save local...
            string fileName = string.Format(@"c:\temp\{0}_{1}_{2}.json", this.GetType().Name, this.ApplicationId, this.Request.SubjectRequestId);
            System.IO.File.AppendAllText(fileName, json);

            if (zipAndLock)
            {
                BaseApplicationMessage msg = (BaseApplicationMessage)this.Request;
                msg.Code = password;

                MemoryStream memStreamIn = new MemoryStream();
                System.IO.Stream fs = new System.IO.FileStream(fileName, FileMode.Open);
                MemoryStream zipped = Utility.CreateToMemoryStream(fs, this.Request.SubjectRequestId.ToString(), password);
                fs.Close();

                fileName = string.Format(@"c:\temp\{0}_{1}_{2}_encrypted.zip", this.GetType().Name, this.ApplicationId, this.Request.SubjectRequestId);
                byte[] byteArrayOut = zipped.ToArray();
                File.WriteAllBytes(fileName, byteArrayOut);

                ei.FileType = "zip";
            }
            else
            {
                ei.FileType = "json";
            }

            //upload to azure...
            StorageContext.Current.TenantId = this.Request.TenantId;
            string url = StorageContext.Current.UploadExportBlob(this.ApplicationId, fileName);
            ei.Urls.Add(url);

            return ei;
        }

        public virtual void Discover(DateTime? checkPoint)
        {
            EncryptionContext ctx = EncryptionContext.CreateForApplication(this.ApplicationId);

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
                        discoverMsg.SystemId = GDPRCore.Current.GetSystemId();
                        discoverMsg.ProcessorId = GDPRCore.Current.GetSystemId();

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
                discoverMsg.SystemId = GDPRCore.Current.GetSystemId();
                discoverMsg.ProcessorId = GDPRCore.Current.GetSystemId();

                MessageHelper.SendMessage(discoverMsg, ctx);
            }
        }

        public abstract List<Record> GetAllRecordTypes();

        public virtual List<EntityPropertyTypeBase> GetEntityPropertyDefinitions()
        {
            List<EntityPropertyTypeBase> types = new List<EntityPropertyTypeBase>();

            //default application properties
            //types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "", Name = "", Type = "checkbox", DefaultValue = "", Category = "", IsMasked = false, IsSecure = false, Description = "" });

            return types;
        }

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

        public virtual void AnonymizeSubject(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectCreateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual RecordCollection SubjectDeleteIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectHoldIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool SubjectHoldOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordCreateIn(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordCreateOut(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordDeleteIn(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordDeleteOut(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual void RecordHold(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordUpdateIn(Record old, Record update)
        {
            throw new NotImplementedException();
        }

        public virtual bool RecordUpdateOut(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual void RecordDelete(Record r)
        {
            throw new NotImplementedException();
        }

        public virtual void RecordUpdate(Record r, GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteRecord(Record r)
        {
            throw new NotImplementedException();
        }
    }
}