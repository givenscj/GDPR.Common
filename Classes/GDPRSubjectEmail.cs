namespace GDPR.Common
{
    using System;
    using System.Collections.Generic;

    public partial class GDPRSubjectEmail : IGDPRSubjectObject
    {
        public System.Guid SubjectId { get; set; }
        public string EmailAddress { get; set; }
        public string Hash { get; set; }
        public bool IsVerified { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool IsPrimary { get; set; }
        public Guid ObjectId { get; set; }
    }
}
