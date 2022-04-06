using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CLDVT2
{
    public class BlobManager
    {
        static string connectionString = "DefaultEndpointsProtocol=https;AccountName=XXX;AccountKey=XXX==;EndpointSuffix=core.windows.net";
        static string containerName = "testbox";

        public static string uploadImage(IFormFile image)
        {
            //identifying the blob using the current date/time is a suitable way of ensuring no duplicates
            BlobClient blob = new BlobContainerClient(connectionString, containerName).GetBlobClient(DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff"));

            blob.Upload(image.OpenReadStream(), true);
            return blob.Uri.AbsoluteUri;
        }

        public static string updateImage(IFormFile image, string uri)
        {
            BlobClient blob = new BlobContainerClient(connectionString, containerName).GetBlobClient(new BlobClient(new Uri(uri)).Name);

            blob.Upload(image.OpenReadStream(), true);
            return blob.Uri.AbsoluteUri;
        }

        public static async void removeBlob(string uri)
        {
            BlobClient blob = new BlobContainerClient(connectionString, containerName).GetBlobClient(new BlobClient(new Uri(uri)).Name);
            await blob.DeleteAsync();
        }
    }
}
