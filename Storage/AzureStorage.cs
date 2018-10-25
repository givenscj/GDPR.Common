using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Storage
{
    public class AzureStorage : Storage
    {
        public override string UploadBlob(Guid applicationId, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            CloudStorageAccount storageAccount;

            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            try
            {
                if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = null;

                    // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(applicationId.ToString());

                    if (!cloudBlobContainer.Exists())
                        cloudBlobContainer.Create();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };

                    cloudBlobContainer.SetPermissions(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fi.Name);
                    cloudBlockBlob.UploadFromFile(fileName, null, null, null);
                    return cloudBlockBlob.Uri.ToString();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}