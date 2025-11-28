Galeriq API – Containerized (.NET 8 + Docker or Podman)

This project is a .NET API designed to run easily inside containers using either:

Docker (most common)

Podman (Docker alternative, rootless mode supported)

The API supports:

- SQL Server

- Azure Service Bus

- Azure Key Vault

- Swagger UI

- Rate limiting

- Automatic HTTP-only mode in containers

📌 Running the API locally (without container)
dotnet run


Swagger available at:

https://localhost:<port>/swagger

🐳 Running with Docker
1. Build the image
docker build -t galeriqapi:test .

2. Run using Key Vault (Azure recommended)

Provide:

KEY_VAULT_URL

Azure credentials for Managed Identity (if needed)

HTTP binding

Development environment for Swagger

docker run -p 8080:5000 \
  -e KEY_VAULT_URL="https://<your-vault>.vault.azure.net/" \
  -e AZURE_TENANT_ID=$AZURE_TENANT_ID \
  -e AZURE_CLIENT_ID=$AZURE_CLIENT_ID \
  -e AZURE_CLIENT_SECRET=$AZURE_CLIENT_SECRET \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:5000 \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  galeriqapi:test


Open Swagger:

http://localhost:8080/swagger

3. Run using Environment Variables (local or non-Azure hosting)
docker run -p 8080:5000 \
  -e DB_CONNECTION="Server=server;Database=db;User Id=user;Password=pw;" \
  -e SB_CONNECTION="Endpoint=sb://...." \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:5000 \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  galeriqapi:test

🫙 Running with Podman [For older machines that don't support Docker]
1. Build the image
podman build -t galeriqapi:test .

2. Run using Key Vault
podman run -p 8080:5000 `
  -e KEY_VAULT_URL="https://<your-vault>.vault.azure.net/" `
  -e AZURE_TENANT_ID=$env:AZURE_TENANT_ID `
  -e AZURE_CLIENT_ID=$env:AZURE_CLIENT_ID `
  -e AZURE_CLIENT_SECRET=$env:AZURE_CLIENT_SECRET `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS=http://+:5000 `
  -e DOTNET_RUNNING_IN_CONTAINER=true `
  galeriqapi:test


Open Swagger:

http://localhost:8080/swagger

3. Run using environment variables
podman run -p 8080:5000 `
  -e DB_CONNECTION="Server=server;Database=db;User Id=user;Password=pw;" `
  -e SB_CONNECTION="Endpoint=sb://...." `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS=http://+:5000 `
  -e DOTNET_RUNNING_IN_CONTAINER=true `
  galeriqapi:test

🔐 How Secrets Are Loaded

The API retrieves secrets in this order:

1️⃣ Azure Key Vault (if KEY_VAULT_URL is set)

galeriqDB → database connection
galeriqSB → service bus connection

2️⃣ Environment Variables (local + non-Azure)

DB_CONNECTION
SB_CONNECTION

3️⃣ Default local fallback

Local SQL Express connection (dev only)

📘 Accessing the API

Inside a container, the API is HTTP-only:

http://localhost:8080/
http://localhost:8080/swagger

## JIRA
Link to board: https://galeriq.atlassian.net/jira/software/projects/MBA/boards/1

