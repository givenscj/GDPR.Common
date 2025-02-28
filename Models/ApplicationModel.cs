﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GDPR.Common.Models
{
    public class ApplicationModel
    {
        public Guid ProcessorId { get; set; }
        public string ProcessorName { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public Guid ApplicationId { get; set; }
        public bool IsRedirect { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AuthenticationTypeId { get; set; }
        public Guid ApplicationTemplateId { get; set; }
        public string TemplateName { get; set; }
        public int Tier { get; set; }
        [Required]
        public string PrivacyUrl { get; set; }
        [Required]
        public string TermsUrl { get; set; }
        public string EndPointUrl { get; set; }
        public string ApplicationName { get; set; }
        public string DeleteMessage { get; set; }
        public string UpdateMessage { get; set; }
        public string HoldMessage { get; set; }
        public string FullName { get; set; }
        public string PublicKey { get; set; }
        public string ApplicationType { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string LongName { get; set; }
        public string Version { get; set; }
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public bool EnableOAuthFallback { get; set; }
        public int SystemKeyVersion { get; set; }
        public DateTime ChangeDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
