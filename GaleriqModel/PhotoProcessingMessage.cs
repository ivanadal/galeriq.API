namespace Galeriq.Model
{
    public class PhotoProcessingMessage
    {
        public Guid PhotoId { get; set; }
        public Guid GalleryId { get; set; }
        public string BlobUrl { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
