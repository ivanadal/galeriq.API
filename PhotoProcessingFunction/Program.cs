using Galeriq.Data;
using Galeriq.PhotoProcessor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
     .ConfigureServices((context, services) =>
     {
         var connectionString = context.Configuration.GetConnectionString("SqlConnection")
            ?? context.Configuration["SqlConnection"]; // fallback for local.settings.json

         services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(connectionString));

         services.AddScoped<IPhotoProcessor, PhotoProcessor>();
     })
    .Build();

host.Run();
