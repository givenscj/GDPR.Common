namespace GDPR.Util.Data
{
    using System;
    using System.Collections.Generic;

    public partial class GDPRSubjectAddress
    {
        public System.Guid AddressId { get; set; }
        public string Raw { get; set; }
        public System.Guid SubjectId { get; set; }
        public bool IsVerified { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<bool> IsPrimary { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
    }
}
