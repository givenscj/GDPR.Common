﻿using System;

namespace GDPR.Common.Models
{
    public class ApplicationModel
    {
        public Guid ProcessorId { get; set; }
        public string ProcessorName { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ApplicationTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string PrivacyUrl { get; set; }
        public string TermsUrl { get; set; }
        public string ApplicationName { get; set; }
        public string PublicKey { get; set; }
        public string ApplicationType { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
