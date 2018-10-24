using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Exceptions
{
    public class GDPRException : Exception
    {
        public string Url { get; set; }

        public GDPRException(string message)
            : base(message)
        {

        }

        public GDPRException(string message, string url)
            : base(message)
        {
            this.Url = url;
        }
    }
}
