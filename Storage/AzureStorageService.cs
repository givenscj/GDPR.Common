﻿using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Common.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Services
{
    public class AzureStorageService : StorageService, IStorageService
    {
        string _url;
        string _key;

        public override string Url
        {
            get
            {
                return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", Configuration.ResourcePrefix.ToLower() + Configuration.StorageAccountName, Key);
            }
            set
            {
                _url = value;
            }
        }

        string _token;

        public override string Key
        {
            get
            {
                if (Configuration.IsManaged && !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
                {
                    if (string.IsNullOrEmpty(_token))
                    {
                        string token = Utility.GetMSIToken("", "https://management.azure.com", "").Result;
                        dynamic keys = GetStorageKeysAsync(token).Result;
                        _token = keys.keys[0].value;
                        return _token;
                    }
                    else
                        return _token;
                }
                else
                    return Configuration.StorageAccountKey;
            }
            set
            {
                _key = value;
            }
        }

        internal async Task<dynamic> GetStorageKeysAsync(string token)
        {
            var uri = new Uri($"{Constants.AzureManagementApi}/subscriptions/{Configuration.SubscriptionId}/resourceGroups/{Configuration.ResourceGroupName}/providers/Microsoft.Storage/storageAccounts/{Configuration.ResourcePrefix + Configuration.StorageAccountName}/listKeys?api-version=2016-01-01");
            var content = new StringContent(string.Empty, Encoding.UTF8, "text/html");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", token);

                using (var response = await httpClient.PostAsync(uri, content))
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    var keys = JsonConvert.DeserializeObject(responseText);
                    return keys;
                }
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

        public string UploadBlob(BlobContext ctx)
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
                        cloudBlobContainer = cloudBlobClient.GetContainerReference(ctx.TenantId.ToString());

                        if (!cloudBlobContainer.Exists())
                            cloudBlobContainer.Create();
                    }
                    catch (Exception ex)
                    {
                        throw new GDPRException($"Container creation failed: {ex.Message}");
                    }

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };

                    cloudBlobContainer.SetPermissions(permissions);

                    byte[] data = ctx.Data;
                    string name = ctx.Name;

                    if (ctx.FileInfo != null)
                    {
                        name = ctx.FileInfo.Name;
                        data = File.ReadAllBytes(ctx.FileInfo.FullName);

                        if (string.IsNullOrEmpty(ctx.Name))
                            ctx.Name = ctx.FileInfo.Name;
                    }

                    //tenantid is the container...
                    //name should be in format of tenantid\applicationid\subjectrequestid.blah...
                    name = string.Format("{0}\\{1}.{2}.{3}", ctx.ApplicationId, ctx.ApplicationRequestId, ctx.Type, ctx.Name);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(name);
                    cloudBlockBlob.UploadFromByteArray(data, 0, data.Length);
                    return cloudBlockBlob.Uri.ToString();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                throw new GDPRException($"Blob upload failed: {ex.Message}");
            }

            return null;
        }

        public string UploadBlob(FileInfo fi)
        {
            BlobContext ctx = new BlobContext();
            ctx.FileInfo = fi;
            ctx.TenantId = GDPRCore.Current.GetSystemId();
            ctx.Type = "General";
            return UploadBlob(ctx);
        }

        public string UploadBlob(string containerName, byte[] fileBytes, string name)
        {
            BlobContext ctx = new BlobContext();
            ctx.Data = fileBytes;
            ctx.Name = name;
            ctx.TenantId = GDPRCore.Current.GetSystemId();
            ctx.Type = containerName;
            return UploadBlob(ctx);
        }

        public string UploadExportBlob(Guid applicationId, string fileName)
        {
            Guid TenantId = GDPRCore.Current.GetApplicationTenantId(applicationId);

            if (TenantId == Guid.Empty)
                TenantId = this.TenantId;

            FileInfo fi = new FileInfo(fileName);
            BlobContext ctx = new BlobContext();
            ctx.ApplicationId = applicationId;
            ctx.FileInfo = fi;
            ctx.TenantId = TenantId;
            ctx.Type = "Export";
            return UploadBlob(ctx);
        }

        public string UploadBlob(string name, byte[] data)
        {
            BlobContext ctx = new BlobContext();
            ctx.Data = data;
            ctx.Name = name;
            ctx.TenantId = GDPRCore.Current.GetSystemId();
            ctx.Type = "General";
            return UploadBlob(ctx);
        }
    }
}
