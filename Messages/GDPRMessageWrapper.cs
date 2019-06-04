using System;

namespace GDPR.Common.Messages
{
    public class GDPRMessageWrapper
    {
        public BaseProcessor Source { get; set; }
        public string ApplicationId { get; set; }
        public string SystemId { get; set; }
        public string Authentication { get; set; }
        public string QueueUri { get; set; }
        public string EventHubName { get; set; }
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public string Check { get; set; }
        public int Retries { get; set; }
        public bool IsSystem { get; set; }
        public bool IsError { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsCompressed { get; set; }
        public int KeyVersion { get; set; }
        public bool IsSigned { get; set; }
        public string ErrorMessage { get; set; }
        public string Object { get; set; }
        public string OffSet { get; set; }
        public DateTime MessageDate { get; set; }
        public DateTime OriginalMessageDate { get; set; }
    }
}
