using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevconAzure.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.IO;

namespace DevconAzure.Operations
{
    
    public class BlobOperations
    {
        private static CloudBlobContainer orderBlobContainer;
        public BlobOperations()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["webjobstorage"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get the Blob reference
            orderBlobContainer = blobClient.GetContainerReference("orders");
            //Create Blob Container if not exist
            orderBlobContainer.CreateIfNotExists();
        }

        public async Task<CloudBlockBlob> UploadBlob(HttpPostedFileBase orderFile)
        {
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(orderFile.FileName);
            //Get a Blob Reference
            CloudBlockBlob orderBlob = orderBlobContainer.GetBlockBlobReference(blobName);

            //Uploading a local file and Create the Blob
            using (var fs = orderFile.InputStream)
            {
                await orderBlob.UploadFromStreamAsync(fs);
            }

            return orderBlob;
        }

    }
}