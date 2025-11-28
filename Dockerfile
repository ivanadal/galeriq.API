# See https://aka.ms/customizecontainer to learn how to customize your debug container.

# Use .NET 10 runtime for final image and SDK for build
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first and restore
COPY ["GaleriqAPI/Galeriq.GaleriqAPI.csproj", "GaleriqAPI/"]
RUN dotnet restore "GaleriqAPI/Galeriq.GaleriqAPI.csproj"

# Copy the rest of the files
COPY . .
WORKDIR "/src/GaleriqAPI"
RUN dotnet build "Galeriq.GaleriqAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Galeriq.GaleriqAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Galeriq.GaleriqAPI.dll"]
