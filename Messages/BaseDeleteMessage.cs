﻿using GDPR.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Messages
{
    public class BaseDeleteMessage : BaseApplicationMessage
    {
        public DeleteInfo Info { get; set; }
    }
}
