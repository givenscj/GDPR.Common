using GDPR.Common.Enums;
using System;

namespace GDPR.Common.Classes
{
    public class ErrorContext
    {
        public Exception Exception { get; set; }
        public string IpAddress { get; set; }
        public LogLevel LogLevel { get; set; }
        public bool CanEmail { get; set; }
        public SecurityContext SecurityContext { get; set; }
    }
}
