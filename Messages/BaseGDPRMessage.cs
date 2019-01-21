using GDPR.Common.Data;
using GDPR.Common.Enums;
using System;
using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Applications;
using GDPR.Common.Exceptions;

namespace GDPR.Common.Messages
{
    public partial class BaseGDPRMessage
    {
        public BaseGDPRMessage()
        {
            this.Retries = 0;
            this.CreateDate = DateTime.Now;
        }

        public bool IsSystem { get; set; }
        public int Retries { get; set; }
        public GDPRSubject Subject { get; set; }
        public SecurityContext Context { get; set; }
        public Guid TenantId { get; set; }
        public Guid SystemId { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationClass { get; set; }
        public Guid ProcessorId { get; set; }
        public Guid SubjectRequestId { get; set; }
        public BaseGDPRApplication Instance { get; set; }
        public BaseSubjectRequest SubjectRequest { get; set; }

        public MessageDirection Direction { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual bool Process()
        {
            try
            { 
                CreateSecurityContext();

                //initalize the Application stub
                Type pType = System.Type.GetType(ApplicationClass);
                Instance = (BaseGDPRApplication)Activator.CreateInstance(pType);

                //need to set the application id BEFORE init to get the proper properties
                Instance.ApplicationId = this.ApplicationId;
                Instance.Init();

                //authorize the app for this request processing...
                Instance.Authorize();

                EncryptionContext ctx = EncryptionContext.CreateForApplication(this.ApplicationId);
                this.Context.Encryption = ctx;

                //fire the request
                Instance.ProcessRequest((BaseApplicationMessage)this, ctx);

                return true;
            }
            catch (Exception ex)
            {
                GDPRCore.Current.ErrorSubjectRequest(ex, this);
                throw new GDPRException("Application is not active", this.Context);
            }

            return false;
        }

        public void CreateSecurityContext()
        {
            if (this.Context == null)
            {
                this.Context = new SecurityContext();
                this.Context.ProcessorId = this.ProcessorId;
                this.Context.TenantId = this.TenantId;
                this.Context.ApplicationId = this.ApplicationId;

                if (this.Subject != null)
                    this.Context.SubjectId = this.Subject.SubjectId;
            }
        }
    }
}
