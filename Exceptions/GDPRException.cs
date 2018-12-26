using GDPR.Common.Classes;
using System;

namespace GDPR.Common.Exceptions
{
    public class GDPRException : Exception
    {
        public SecurityContext SecurityContext { get; set; }
        public EncryptionContext EncryptionContext { get; set; }
        public string Url { get; set; }

        public GDPRException(string message, SecurityContext ctx)
            : base(message)
        {
            this.SecurityContext = ctx;
        }

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
