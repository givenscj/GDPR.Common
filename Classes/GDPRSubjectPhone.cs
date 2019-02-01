namespace GDPR.Common
{
    using System;

    public partial class GDPRSubjectPhone : IGDPRSubjectObject
    {
        public System.Guid PhoneId { get; set; }
        public System.Guid SubjectId { get; set; }
        public System.Guid ObjectId { get; set; }
        public bool IsVerified { get; set; }
        public int PhoneTypeId { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<bool> IsPrimary { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public string Raw { get; set; }
    }
}
