namespace GDPR.Common.Data
{
    using System;
    using System.Collections.Generic;

    public partial class BaseApplication
    {
        public BaseApplication()
        {
        }

        public System.Guid ApplicationId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public string ProcessorClass { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ChangeDate { get; set; }
        public Nullable<System.Guid> ProcessorId { get; set; }
        public System.Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }
        public string TermsUrl { get; set; }
        public string PrivacyUrl { get; set; }
        public string EndPointUrl { get; set; }
        public byte[] Image { get; set; }
        public string DeleteMessage { get; set; }
        public System.Guid ApplicationTemplateId { get; set; }
        public System.Guid TenantId { get; set; }
        public Nullable<System.Guid> AuthenticationTypeId { get; set; }
    }
}
