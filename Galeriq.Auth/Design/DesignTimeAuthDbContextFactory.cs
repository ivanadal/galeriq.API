using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeAuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        var conn = "Server=localhost,1434;Database=GaleriqAuthDb;User Id=sa;Password=Auth_password123;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(conn);
        return new AuthDbContext(optionsBuilder.Options);
    }
}
