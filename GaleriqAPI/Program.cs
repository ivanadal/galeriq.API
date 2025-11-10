using Azure.Identity;
using CollectionsAPI.Services;
using Galeriq.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<ServiceBusSettings>(
            builder.Configuration.GetSection("Azure:ServiceBus"));
        // Retrieve Key Vault name from config
        var keyVaultName = builder.Configuration["KeyVaultName"];
        if (string.IsNullOrEmpty(keyVaultName))
            throw new InvalidOperationException("KeyVaultName is not configured.");

        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

        // Add Azure Key Vault secrets to configuration
        builder.Configuration.AddAzureKeyVault(
            keyVaultUri,
            new DefaultAzureCredential());

        // Now you can directly access secrets as if they were in appsettings.json
        var connectionString = builder.Configuration["galeriqDB"];

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}