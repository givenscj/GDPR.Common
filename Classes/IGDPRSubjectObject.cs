using System;
using System.Collections.Generic;

namespace GDPR.Common
{
    public interface IGDPRSubjectObject
    {
        Guid SubjectId { get; set; }
        Guid ObjectId { get; set; }
        string Hash { get; set; }
        bool IsVerified { get; set; }
        Nullable<System.DateTime> VerifiedDate { get; set; }
    }
}
