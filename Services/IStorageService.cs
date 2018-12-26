using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace GDPR.Common.Services
{
    public interface IStorageService
    {
        object StartLogBlob(Guid applicationId);
        string UploadBlob(FileInfo fi);
        string UploadBlob(string name, byte[] data);
        string UploadBlob(Guid applicationId, string fileName);
        string UploadBlob(string containerName, byte[] fileBytes, string name);
        string Url { get; set; }
        string Key { get; set; }
    }
}
