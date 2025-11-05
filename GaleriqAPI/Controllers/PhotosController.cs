using Azure.Core;
using CollectionsAPI.Services;
using Galeriq.Data;
using Galeriq.Data.Entities;
using Galeriq.Model;
using Microsoft.AspNetCore.Mvc;

namespace CollectionsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BlobStorageService _blobStorageService;
        private readonly ServiceBusQueueService _queueService;

        public PhotosController(AppDbContext context, BlobStorageService blobStorageService, ServiceBusQueueService queueService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _queueService = queueService;
        }

        // STEP 1: Request SAS URL for direct upload
        [HttpPost("upload-url")]
        public IActionResult GetUploadUrl([FromBody] UploadRequest request)
        {
            var sasUrl = _blobStorageService.GetSasUrl(
                userId: request.UserId,
                galleryId: request.GalleryId,
                fileName: request.FileName,
                contentType: request.ContentType
            );

            return Ok(new
            {
                uploadUrl = sasUrl,
                blobPath = $"photos/{request.UserId}/{request.GalleryId}/original/{request.FileName}"
            });
        }

        // STEP 2: After upload is done, save photo metadata
        [HttpPost]
        public async Task<IActionResult> SavePhotoMetadata([FromBody] Photo photo)
        {
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            var msg = new PhotoProcessingMessage
            {
                PhotoId = photo.Id,
                GalleryId = photo.Id,
                BlobUrl = photo.OriginalUrl,
                UserId = photo.UserId
            };

            await _queueService.SendMessageAsync(msg);

            return Ok(photo);
        }
    }

    public class UploadRequest
    {
        public Guid UserId { get; set; }
        public Guid GalleryId { get; set; }
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
    }
}
