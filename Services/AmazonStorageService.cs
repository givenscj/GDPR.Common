using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using GDPR.Common.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace GDPR.Common.Services
{
    public class AmazonStorageService : IStorageService
    {
        string _url;
        string _key;

        public string Url
        {
            get
            {
                return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", Configuration.StorageAccountName, Configuration.StorageAccountKey);
            }
            set
            {
                _url = value;
            }
        }
        public string Key
        {
            get
            {
                return Configuration.StorageAccountKey;
            }
            set
            {
                _key = value;
            }
        }

        public object StartLogBlob(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public string UploadBlob(FileInfo fi)
        {
            return UploadBlob(GDPRCore.Current.GetSystemId().ToString(), File.ReadAllBytes(fi.FullName), fi.Name);
        }

        public string UploadBlob(string name, byte[] data)
        {
            return UploadBlob(GDPRCore.Current.GetSystemId().ToString(), data, name);
        }

        public string UploadBlob(Guid applicationId, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return UploadBlob(applicationId.ToString(), File.ReadAllBytes(fi.FullName), fi.Name);
        }

        public string UploadBlob(string containerName, byte[] fileBytes, string name)
        {
            RegionEndpoint endpoint = RegionEndpoint.GetBySystemName(Configuration.StorageRegion);
            AmazonS3Client s3Client = new AmazonS3Client(endpoint);
            var fileTransferUtility = new TransferUtility(s3Client);
            fileTransferUtility.Upload(name, containerName);

            /*
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = containerName,
                FilePath = filePath,
                StorageClass = S3StorageClass.StandardInfrequentAccess,
                PartSize = 6291456, // 6 MB.
                Key = keyName,
                CannedACL = S3CannedACL.PublicRead
            };
            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

            fileTransferUtility.Upload(fileTransferUtilityRequest);
            */

            return "";
        }
    }
}
