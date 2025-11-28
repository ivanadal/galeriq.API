using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using CollectionsAPI.Services;
using Galeriq.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services first
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ---- KEY VAULT / ENV ----
        var keyVaultUrl = Environment.GetEnvironmentVariable("KEY_VAULT_URL");

        string? connectionString = null;
        string? serviceBusConnection = null;

        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            try
            {
                KeyVaultSecret secret = client.GetSecret("galeriqDB");
                KeyVaultSecret secretSB = client.GetSecret("galeriqSB");
                connectionString = secret.Value;
                serviceBusConnection = secretSB.Value;
            }
            catch
            {
                Console.WriteLine("Could not get secret from Key Vault, falling back to env...");
            }
        }

        connectionString ??= Environment.GetEnvironmentVariable("DB_CONNECTION")
                            ?? "Server=localhost;Database=mydb;User Id=sa;Password=YourStrong(!)Password;";
        serviceBusConnection ??= Environment.GetEnvironmentVariable("SB_CONNECTION")
                               ?? "Endpoint=sb://localhost.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=secret";

        // ---- DATABASE ----
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // ---- SERVICE BUS ----
        builder.Services.AddSingleton(sp => new ServiceBusClient(serviceBusConnection));

        // ---- RATE LIMITING ----
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name
                                  ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                  ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }));
        });

        // ---- BUILD APP ----
        var app = builder.Build();

        // ---- MIDDLEWARE ----
        if (!string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true"))
        {
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapGet("/", () => "API is running!");

        app.MapControllers();

        app.Run();
    }
}
