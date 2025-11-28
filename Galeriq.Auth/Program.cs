using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext for Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection") ?? 
        "Server=(localdb)\\mssqllocaldb;Database=GaleriqAuthDb;Trusted_Connection=True;");

    // Register the OpenIddict entity sets.
    options.UseOpenIddict();
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");

        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();

        // Register the signing and encryption credentials.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

// Minimal models and DbContext
public class ApplicationUser : IdentityUser { }

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
}
