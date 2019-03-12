namespace GDPR.Common
{
    using System;
    using System.Collections.Generic;

    public partial class GDPRSubjectDna : IGDPRSubjectObject
    {
        public System.Guid SubjectId { get; set; }
        public System.Guid ObjectId { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public bool IsVerified { get; set; }
        public Nullable<System.DateTime> VerifiedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.Guid SubjectDnaId { get; set; }
        public System.Guid SubjectFileId { get; set; }
    }
}
