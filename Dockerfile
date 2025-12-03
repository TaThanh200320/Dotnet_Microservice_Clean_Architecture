# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["DotnetCoreMicroservice.sln", "./"]

# Copy all project files for restore
COPY ["libs/Api/Api.csproj", "libs/Api/"]
COPY ["libs/Application/Application.csproj", "libs/Application/"]
COPY ["libs/Contracts/Contracts.csproj", "libs/Contracts/"]
COPY ["libs/Domain/Domain.csproj", "libs/Domain/"]
COPY ["libs/DynamicQuery/DynamicQuery.csproj", "libs/DynamicQuery/"]
COPY ["libs/FluentConfiguration/FluentConfiguration.csproj", "libs/FluentConfiguration/"]
COPY ["libs/Infrastructure/Infrastructure.csproj", "libs/Infrastructure/"]
COPY ["libs/SharedKernel/SharedKernel.csproj", "libs/SharedKernel/"]
COPY ["libs/Specification/Specification.csproj", "libs/Specification/"]

COPY ["src/Identity/Api/IdentityApi.csproj", "src/Identity/Api/"]
COPY ["src/Identity/Application/IdentityApplication.csproj", "src/Identity/Application/"]
COPY ["src/Identity/Domain/IdentityDomain.csproj", "src/Identity/Domain/"]
COPY ["src/Identity/Infrastructure/IdentityInfrastructure.csproj", "src/Identity/Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Identity/Api/IdentityApi.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/Identity/Api"
RUN dotnet build "IdentityApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "IdentityApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "IdentityApi.dll"]
