namespace GDPR.Common
{
    using System;

    public class GDPRSubjectAddress : BaseAddress, IGDPRSubjectObject
    {
        public System.Guid AddressId { get; set; }
        override public string Raw { get; set; }
        public string Hash { get; set; }
        public System.Guid SubjectId { get; set; }
        public Guid ObjectId { get; set; }
        public bool IsVerified { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<bool> IsPrimary { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        
    }
}
