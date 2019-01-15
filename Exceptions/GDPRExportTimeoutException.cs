using GDPR.Common.Classes;
using System;

namespace GDPR.Common.Exceptions
{
    public class GDPRExportTimeoutException : GDPRException
    {
        public ExportInfo Info { get; set; }
        public GDPRExportTimeoutException(string message) : base(message)
        {

        }
    }
}
