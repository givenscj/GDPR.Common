using GDPR.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public partial class BaseApplicationMessage : BaseGDPRMessage
    {        
        public BaseApplicationMessage()
        {
            
        }

        public bool IsLocked { get; set; }
        public DateTime LockedDate { get; set; }
        public string ApplicationSubjectId { get; set; }
        public Guid SubjectRequestApplicationId { get; set; }
    }
}
