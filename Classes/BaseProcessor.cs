using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class BaseProcessor
    {
        public System.Guid ProcessorId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ProcessorClass { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ApiEndPoint { get; set; }
        public byte[] PublicKey { get; set; }
    }
}
