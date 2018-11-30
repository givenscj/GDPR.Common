using System;

namespace GDPR.Common.Messages
{
    public partial class BaseApplicationMessage : BaseGDPRMessage
    {        
        public BaseApplicationMessage()
        {
            
        }

        public int Version { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockedDate { get; set; }
        public string ApplicationSubjectId { get; set; }
        public Guid SubjectRequestApplicationId { get; set; }
    }
}
