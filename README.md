# Galeriq API – Containerized (.NET 8 + Docker/Podman)

This project is a **.NET API** designed to run easily inside containers using either:

* **Docker** (most common)
* **Podman** (Docker alternative, rootless mode supported)

The API supports:

* SQL Server
* Azure Service Bus
* Azure Key Vault
* Swagger UI
* Rate limiting
* Automatic HTTP-only mode in containers

---

## 📌 Running the API locally (without container)

```bash
dotnet run
```

Swagger available at:

```
https://localhost:<port>/swagger
```

---

## 🐳 Running with Docker

### 1. Build the image

```bash
docker build -t galeriqapi:test .
```

### 2. Run using Azure Key Vault (recommended)

Provide:

* `KEY_VAULT_URL`
* Azure credentials for Managed Identity (if needed)
* HTTP binding
* Development environment for Swagger

```bash
docker run -p 8080:5000 \
  -e KEY_VAULT_URL="https://<your-vault>.vault.azure.net/" \
  -e AZURE_TENANT_ID=$AZURE_TENANT_ID \
  -e AZURE_CLIENT_ID=$AZURE_CLIENT_ID \
  -e AZURE_CLIENT_SECRET=$AZURE_CLIENT_SECRET \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:5000 \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  galeriqapi:test
```

Open Swagger:

```
http://localhost:8080/swagger
```

### 3. Run using Environment Variables (local or non-Azure hosting)

```bash
docker run -p 8080:5000 \
  -e DB_CONNECTION="Server=server;Database=db;User Id=user;Password=pw;" \
  -e SB_CONNECTION="Endpoint=sb://...." \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:5000 \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  galeriqapi:test
```

---

## 🫙 Running with Podman

### 1. Build the image

```bash
podman build -t galeriqapi:test .
```

### 2. Run using Azure Key Vault

```powershell
podman run -p 8080:5000 `
  -e KEY_VAULT_URL="https://<your-vault>.vault.azure.net/" `
  -e AZURE_TENANT_ID=$env:AZURE_TENANT_ID `
  -e AZURE_CLIENT_ID=$env:AZURE_CLIENT_ID `
  -e AZURE_CLIENT_SECRET=$env:AZURE_CLIENT_SECRET `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS=http://+:5000 `
  -e DOTNET_RUNNING_IN_CONTAINER=true `
  galeriqapi:test
```

Open Swagger:

```
http://localhost:8080/swagger
```

### 3. Run using Environment Variables

```powershell
podman run -p 8080:5000 `
  -e DB_CONNECTION="Server=server;Database=db;User Id=user;Password=pw;" `
  -e SB_CONNECTION="Endpoint=sb://...." `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS=http://+:5000 `
  -e DOTNET_RUNNING_IN_CONTAINER=true `
  galeriqapi:test
```

---

## 🔐 How Secrets Are Loaded

The API retrieves secrets in this order:

1️⃣ **Azure Key Vault** (if `KEY_VAULT_URL` is set)

* `galeriqDB` → database connection
* `galeriqSB` → service bus connection

2️⃣ **Environment Variables** (local + non-Azure)

* `DB_CONNECTION`
* `SB_CONNECTION`

3️⃣ **Default local fallback**

* Local SQL Express connection (dev only)

---

## 🌐 Deploying to Azure Web App for Containers

To run on **Azure**:

1. **Create resources**:

   * **Resource Group**
   * **Azure Container Registry (ACR)**
   * **Web App for Containers**

2. **Assign roles to your service principal (App Registration)**:

   * `Contributor` → Web App or Resource Group
   * `AcrPush` → ACR (if using private registry)

3. **Set Web App environment variables**:

   * `WEBSITES_PORT=80` (matches Dockerfile)
   * Optional: DB/SB connection strings if not using Key Vault

4. **Push and deploy container**:

   * GitHub Actions workflow will handle building, pushing, and deploying the container automatically.

---

## 📘 Accessing the API

Inside a container, the API is **HTTP-only**:

```
http://localhost:8080/
http://localhost:8080/swagger
```

---

## 📝 JIRA

Link to board: [Galeriq JIRA](https://galeriq.atlassian.net/jira/software/projects/MBA/boards/1)
