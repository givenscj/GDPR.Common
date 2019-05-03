namespace GDPR.Common
{
    using GDPR.Common.Enums;
    using System;
    using System.Collections.Generic;

    public partial class GDPRSubjectIdentity : IGDPRSubjectObject
    {
        public System.Guid GovernmentIdentityTypeId { get; set; }
        public GovernmentIdentity GovernmentIdentityType { get; set; }
        public System.Guid SubjectId { get; set; }
        public System.Guid ObjectId { get; set; }
        public string IdNumber { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public bool IsVerified { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.Guid SubjectIdentityId { get; set; }
    }
}
