namespace Galeriq.Data.Entities
{
    public class Photo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GalleryId { get; set; }
        public Guid UserId { get; set; }
        public string OriginalUrl { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long SizeBytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; }
    }
}
