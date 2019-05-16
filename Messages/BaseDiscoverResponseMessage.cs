using System.Collections.Generic;

namespace GDPR.Common.Messages
{
    public class BaseDiscoverResponsesMessage : BaseApplicationMessage
    {
        public List<GDPRSubject> Subjects { get; set; }
        public int OffSet { get; set; }
        
        public override bool Process()
        {
            return base.Process();
        }

        
    }
}
