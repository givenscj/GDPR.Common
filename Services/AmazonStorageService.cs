using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using GDPR.Common.Classes;
using GDPR.Common.Core;
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

        public string UploadExportBlob(Guid applicationId, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return UploadBlob(applicationId.ToString(), File.ReadAllBytes(fi.FullName), fi.Name);
        }

        public string UploadBlob(string containerName, byte[] fileBytes, string name)
        {
            RegionEndpoint endpoint = RegionEndpoint.GetBySystemName(Configuration.StorageRegion);

            var options = new CredentialProfileOptions
            {
                AccessKey = Configuration.StorageAccountKey,
                SecretKey = Configuration.StorageAccountSecret
            };

            CredentialProfile profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("basic_profile", options);
            profile.Region = endpoint;

            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);

            AWSCredentials awsCredentials = profile.GetAWSCredentials(profile.CredentialProfileStore);
            AmazonS3Client s3Client = new AmazonS3Client(awsCredentials, endpoint);
            
            var fileTransferUtility = new TransferUtility(s3Client);

            var bucket = s3Client.PutBucket(new PutBucketRequest { BucketName = containerName });
            s3Client.DeleteObject(new DeleteObjectRequest() { BucketName = containerName, Key = name });

            MemoryStream ms = new MemoryStream();
            ms.Read(fileBytes, 0, fileBytes.Length);
           
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = containerName,
                InputStream = ms,
                ContentType = "application/octet-stream",
                StorageClass = S3StorageClass.StandardInfrequentAccess,
                PartSize = 6291456, // 6 MB.
                Key = name,
                CannedACL = S3CannedACL.PublicRead
            };

            //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
            //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

            fileTransferUtility.Upload(fileTransferUtilityRequest);

            return "";
        }

        public string UploadBlob(BlobContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
