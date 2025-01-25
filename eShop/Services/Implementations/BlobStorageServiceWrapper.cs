using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace eShop.Services
{
    public class BlobStorageServiceWrapper : IBlobStorageServiceWrapper
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageServiceWrapper(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream content, string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        }

        public string GenerateBlobSasUri(string containerName, string blobName, BlobSasPermissions permissions,
            TimeSpan expiry)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.Exists())
            {
                throw new FileNotFoundException($"Blob '{blobName}' not found.");
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
            };

            sasBuilder.SetPermissions(permissions);
            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}