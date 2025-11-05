namespace Galeriq.Data.Entities
{
    public class Gallery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Photo> Photos { get; set; } = new();
    }
}
