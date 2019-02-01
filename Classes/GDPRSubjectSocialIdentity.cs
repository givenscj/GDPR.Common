using System;

namespace GDPR.Common
{
    public partial class GDPRSubjectSocialIdentity : IGDPRSubjectObject
    {
        public string Username { get; set; }
        public string UserId { get; set; }

        public string Type { get; set; }

        public bool IsVerified { get; set; }

        public DateTime VerifiedDate { get; set; }
        public Guid SubjectId { get; set; }
        public Guid ObjectId { get; set; }
    }
}
