using GDPR.Common.Classes;
using System;
using System.IO;

namespace GDPR.Common.Services
{
    public class StorageService : IStorageService
    { 
        public Guid TenantId { get; set; }
        virtual public string Url { get; set; }
        virtual public string Key { get; set; }

        public virtual object StartLogBlob(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadBlob(FileInfo fi)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadBlob(BlobContext ctx)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadBlob(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadBlob(string containerName, byte[] data, string name)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadExportBlob(Guid applicationId, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
