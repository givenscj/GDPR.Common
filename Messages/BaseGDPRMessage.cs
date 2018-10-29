using GDPR.Common.Data;
using GDPR.Common.Enums;
using System;
using GDPR.Common.Classes;

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
        public Guid ProcessorId { get; set; }
        public Guid SubjectRequestId { get; set; }
        public BaseSubjectRequest SubjectRequest { get; set; }

        public MessageDirection Direction { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual bool Process()
        {
            return true;
        }

        public void CreateSecurityContext()
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
