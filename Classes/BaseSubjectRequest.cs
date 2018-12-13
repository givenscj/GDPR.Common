namespace GDPR.Common.Data
{
    using System;
    using System.Collections.Generic;

    public partial class BaseSubjectRequest
    {
        public BaseSubjectRequest()
        {
        }

        public string ContactData { get; set; }
        public int PinVersion { get; set; }

        public System.Guid SubjectRequestId { get; set; }
        public System.Guid SubjectId { get; set; }
        public string Type { get; set; }
        public bool IsComplete { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool IsExternal { get; set; }
        public string ErrorMessage { get; set; }
        public string MessageData { get; set; }
        public string EventHubOffset { get; set; }
        public int Retries { get; set; }
        public string SubjectData { get; set; }
        public bool HasUnverifiedData { get; set; }
        public Nullable<System.Guid> UserTransactionId { get; set; }
    }
}
