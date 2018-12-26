using GDPR.Common.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace GDPR.Common.Services
{
    public class AzureStorageService : IStorageService
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
            CloudStorageAccount strAcc = GetStorageAccount();
            CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

            //Setup our container we are going to use and create it.
            CloudBlobContainer container = blobClient.GetContainerReference("importlogs");
            container.CreateIfNotExistsAsync();

            // Build my typical log file name.
            DateTime date = DateTime.Today;
            DateTime dateLogEntry = DateTime.Now;
            // This creates a reference to the append blob we are going to use.
            CloudAppendBlob appBlob = container.GetAppendBlobReference(
                string.Format("{0}{1}", date.ToString("yyyyMMdd"), ".log"));

            // Now we are going to check if todays file exists and if it doesn't we create it.
            if (!appBlob.Exists())
            {
                appBlob.CreateOrReplace();
            }

            return appBlob;
        }

        public CloudStorageAccount GetStorageAccount()
        {
            CloudStorageAccount storageAccount;
            string storageConnectionString = Url;

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                return storageAccount;

            return null;
        }

        public string UploadBlob(FileInfo fi)
        {
            return UploadBlob(GDPRCore.Current.GetSystemId().ToString(), File.ReadAllBytes(fi.FullName), fi.Name);
        }

        public string UploadBlob(string containerName, byte[] fileBytes, string name)
        {
            CloudStorageAccount storageAccount = GetStorageAccount();

            try
            {
                if (storageAccount != null)
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = null;

                    try
                    {
                        // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                        cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                        if (!cloudBlobContainer.Exists())
                            cloudBlobContainer.Create();
                    }
                    catch (Exception ex)
                    {
                        GDPRCore.Current.Log(ex, GDPR.Common.Enums.LogLevel.Warning);
                    }

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };

                    cloudBlobContainer.SetPermissions(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(name);
                    cloudBlockBlob.UploadFromByteArray(fileBytes, 0, fileBytes.Length);
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

        public string UploadBlob(Guid applicationId, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            return UploadBlob(applicationId.ToString(), File.ReadAllBytes(fi.FullName), fi.Name);
        }

        public string UploadBlob(string name, byte[] data)
        {
            return UploadBlob(GDPRCore.Current.GetSystemId().ToString(), data, name);
        }
    }
}
