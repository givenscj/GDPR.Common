﻿using GDPR.Applications;
using GDPR.Util.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class BaseDiscoverResponsesMessage : BaseApplicationMessage
    {
        public List<GDPRSubject> Subjects { get; set; }
        public int OffSet { get; set; }
        
        public override bool Process()
        {
            return true;
        }

        
    }
}
