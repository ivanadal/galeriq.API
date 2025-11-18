# Galeriq

Repository for the Galeriq photo gallery system. This solution contains an ASP.NET Core API for galleries and photos, a photo processing library, an Azure Function for background processing, and unit tests.

## Projects

- `GaleriqAPI` (project file: `GaleriqAPI/Galeriq.CollectionsAPI.csproj`) - ASP.NET Core Web API exposing gallery and photo endpoints.
- `Galeriq.Data` (`Galeriq.Data/Galeriq.Data.csproj`) - EF Core entities and `AppDbContext`.
- `Galeriq.PhotoProcessor` (`PhotoProcessor/Galeriq.PhotoProcessor.csproj`) - Photo processing library.
- `Galeriq.PhotoProcessingFunction` (`PhotoProcessingFunction/Galeriq.PhotoProcessingFunction.csproj`) - Azure Function that processes photos (uses Service Bus + storage).
- `Galeriq.UnitTests` (`Galeriq.UnitTests/Galeriq.UnitTests.csproj`) - Unit tests.
- `Galeriq.Model` (`GaleriqModel/Galeriq.Model.csproj`) - Shared model types (if present).

## Requirements

- .NET8 SDK
- Docker (for optional local dependencies)
- (Optional) Azure resources if you want to run the function and message queue: Service Bus, Storage Account

## Quick start

1. Clone the repository and open a terminal in the repo root.

2. Restore and build:

```bash
dotnet restore
dotnet build
```

3. (Optional) Start local dependencies with Docker Compose (SQL Server + Azurite):

```bash
docker-compose up -d
```

This will start:
- SQL Server on `localhost:1433` with SA password `Your_password123`.
- Azurite (Azure Storage emulator) on `localhost:10000` (Blob) and `localhost:10001` (Queue).

4. Update `GaleriqAPI/appsettings.json` or set environment variables with the following local values for development when using Docker Compose:

```json
{
 "ConnectionStrings": {
 "DefaultConnection": "Server=localhost,1433;Database=GaleriqDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
 },
 "AzureStorage": {
 "BlobServiceUrl": "http://127.0.0.1:10000/devstoreaccount1",
 "AccountName": "devstoreaccount1",
 "AccountKey": "Eby8vdM02xNoGV...<AzuriteDefaultKey>..."
 },
 "ServiceBus": {
 "ConnectionString": "<your-service-bus-connection-string>",
 "QueueName": "photo-processing"
 }
}
```

Note: Azurite uses a default account name `devstoreaccount1` and a well-known key; check Azurite docs for the exact key to use in local development.

5. Run the API (from repo root):

```bash
dotnet run --project GaleriqAPI/Galeriq.CollectionsAPI.csproj
```

The API will read configuration from `GaleriqAPI/appsettings.json` and environment variables. Update connection strings and Azure settings as needed.

## Configuration

Edit `GaleriqAPI/appsettings.json` or set environment variables for:

- Database connection string for EF Core (e.g. `ConnectionStrings:DefaultConnection`).
- Service Bus connection: `ServiceBus:ConnectionString`, `ServiceBus:QueueName` (used by `ServiceBusQueueService`).
- Storage account or other blob settings used by photo uploads.

Keep secrets out of source control; prefer user secrets or environment variables for local development.

## Running the photo processing function

If you want to run the Azure Function locally:

```bash
cd PhotoProcessingFunction
func start
```

Ensure your local environment has the required Service Bus and Storage connection strings configured. If you're using Azurite for storage locally, point the function storage settings to the Azurite endpoints.

## Tests

Run unit tests:

```bash
dotnet test
```

## Contributing

- Create a new branch for changes.
- Add tests for bug fixes or new features.
- Open a pull request with a clear description.


