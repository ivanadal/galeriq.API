using Galeriq.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Galeriq.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
