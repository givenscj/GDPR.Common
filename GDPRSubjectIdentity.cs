namespace GDPR.Util.Data
{
    using System;
    using System.Collections.Generic;

    public partial class GDPRSubjectIdentity
    {
        public System.Guid GovernmentIdentityTypeId { get; set; }
        public System.Guid SubjectId { get; set; }
        public string IdNumber { get; set; }
        public bool IsVerified { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.Guid SubjectIdentityId { get; set; }
    }
}
