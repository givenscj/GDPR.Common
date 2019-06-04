using System;

namespace GDPR.Common
{
    public partial class GDPRSubjectIpAddress : IGDPRSubjectObject
    {
        public Guid SubjectId { get; set; }
        public Guid ObjectId { get; set; }
        public string IpAddress { get; set; }
        public string Hash { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
}
