using System;
using System.Collections.Generic;

namespace GDPR.Common
{
    public interface IGDPRSubjectObject
    {
        Guid SubjectId { get; set; }

        Guid ObjectId { get; set; }

    }
}
