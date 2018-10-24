using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class Record
    {
        public Guid ApplicationId { get; set; }
        public string Type { get; set; }
        public string RecordId { get; set; }
        public DateTime RecordDate { get; set; }
        public object Object { get; set; }
        public string LinkUrl { get; set; }
        public string Message { get; set; }
        public bool CanDelete { get; set; }

        public bool AnonymizeOnDelete { get; set; }
    }
}
