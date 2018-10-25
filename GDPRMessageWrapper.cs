using GDPR.Utililty.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDPR.Common;

namespace GDPR.Utililty.Messages
{
    public class GDPRMessageWrapper
    {
        public Processor Source { get; set; }
        public string ApplicationId { get; set; }
        public string Authentication { get; set; }
        public string Type { get; set; }
        public int Retries { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string Object { get; set; }
        public string OffSet { get; set; }
        public DateTime MessageDate { get; set; }
        public DateTime OriginalMessageDate { get; set; }
    }
}
