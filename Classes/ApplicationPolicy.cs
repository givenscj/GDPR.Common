namespace GDPR.Common.Data
{
    using System;
    using System.Collections.Generic;

    public partial class BaseApplicationPolicy
    {
        public System.Guid ApplicationPolicyId { get; set; }
        public System.Guid ApplicationId { get; set; }
        public string RecordType { get; set; }
        public int MinRecordAgeDays { get; set; }
        public string Message { get; set; }
        public Nullable<int> Order { get; set; }
        public string Legal { get; set; }
        public string ObjectIgnorePaths { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool AnonymizeRecordOnDelete { get; set; }
    }
}
