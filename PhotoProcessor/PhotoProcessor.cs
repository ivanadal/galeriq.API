using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Galeriq.Data;
using Galeriq.Model;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace Galeriq.PhotoProcessor
{
    public class PhotoProcessor : IPhotoProcessor
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AppDbContext _dbContext;

        public PhotoProcessor(BlobServiceClient blobServiceClient, AppDbContext dbContext)
        {
            _blobServiceClient = blobServiceClient;
            _dbContext = dbContext;
        }

        public async Task ProcessPhotoAsync(PhotoProcessingMessage message)
        {
            var blobUri = new Uri(message.BlobUrl);
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobUri.Segments[1].TrimEnd('/'));
            var blobName = string.Join("", blobUri.Segments.Skip(2));
            var blobClient = containerClient.GetBlobClient(blobName);

            // Download image
            using var imageStream = new MemoryStream();
            await blobClient.DownloadToAsync(imageStream);
            imageStream.Position = 0;

            // Create thumbnail
            using var image = Image.FromStream(imageStream);
            using var thumbnail = image.GetThumbnailImage(300, 300, null, IntPtr.Zero);
            using var thumbStream = new MemoryStream();
            thumbnail.Save(thumbStream, ImageFormat.Jpeg);
            thumbStream.Position = 0;

            // Upload thumbnail
            var thumbBlobName = $"{Path.GetDirectoryName(blobName)}/thumb_{Path.GetFileName(blobName)}";
            var thumbBlobClient = containerClient.GetBlobClient(thumbBlobName);
            await thumbBlobClient.UploadAsync(thumbStream, new BlobHttpHeaders { ContentType = "image/jpeg" });

            // Update database
            var photo = await _dbContext.Photos.FirstOrDefaultAsync(p => p.Id == message.PhotoId);
            if (photo != null)
            {
                photo.ThumbnailUrl = thumbBlobClient.Uri.ToString();
                photo.IsProcessed = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}