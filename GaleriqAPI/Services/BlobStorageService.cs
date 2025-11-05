using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace CollectionsAPI.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "photos";

        public BlobStorageService(IConfiguration config)
        {
            _blobServiceClient = new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public string GetSasUrl(Guid userId, Guid galleryId, string fileName, string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient($"photos/{userId}/{galleryId}/original/{fileName}");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
    }
}
