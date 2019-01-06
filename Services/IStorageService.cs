using GDPR.Common.Classes;
using System;
using System.IO;

namespace GDPR.Common.Services
{
    public interface IStorageService
    {
        object StartLogBlob(Guid applicationId);
        string UploadBlob(FileInfo fi);
        string UploadBlob(BlobContext ctx);
        string UploadBlob(string name, byte[] data);
        string UploadExportBlob(Guid applicationId, string fileName);
        string UploadBlob(string containerName, byte[] data, string name);
        string Url { get; set; }
        string Key { get; set; }
    }
}
