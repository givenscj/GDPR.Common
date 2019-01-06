using System;
using System.IO;

namespace GDPR.Common.Classes
{
    public class BlobContext
    {
        public FileInfo FileInfo { get; set; }
        public byte[] Data { get; set; }
        public Guid TenantId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ApplicationRequestId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
