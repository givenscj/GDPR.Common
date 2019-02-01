using System;

namespace GDPR.Common
{
    public partial class GDPRSubjectIpAddress : IGDPRSubjectObject
    {
        public Guid SubjectId { get; set; }
        public Guid ObjectId { get; set; }
    }
}
