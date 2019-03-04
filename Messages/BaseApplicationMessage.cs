using System;

namespace GDPR.Common.Messages
{
    public partial class BaseApplicationMessage : BaseGDPRMessage
    {        
        public BaseApplicationMessage()
        {
            
        }

        public int Version { get; set; }
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public string Code { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockedDate { get; set; }
        public string ApplicationSubjectId { get; set; }
        public Guid SubjectRequestApplicationId { get; set; }
    }
}
