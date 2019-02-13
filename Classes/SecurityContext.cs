using System;

namespace GDPR.Common.Classes
{
    public class SecurityContext
    {
        public string LanguageId { get; set; }
        public string IpAddress { get; set; }
        public Guid ImpersonatingUserId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool ReadOnly { get; set; }
        public Guid TenantId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ProcessorId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid ParentSubjectId { get; set; }
        public string WebType { get; set; }
        public EncryptionContext Encryption { get; set; }
    }
}
