using CollectionsAPI.Services;
using Galeriq.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ServiceBusSettings>(
builder.Configuration.GetSection("Azure:ServiceBus"));

//var connectionString = context.Configuration.GetConnectionString("SqlConnection")
//           ?? context.Configuration["SqlConnection"]; // fallback for local.settings.json

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));

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
