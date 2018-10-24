using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDPR.Util.Data;

namespace GDPR.Util.Messages
{
    public class BaseExportMessage : BaseApplicationMessage
    {
        public string BlobUrl { get; set; }
        public override bool Process()
        {
            base.Process();

            return true;
        }
    }
}
