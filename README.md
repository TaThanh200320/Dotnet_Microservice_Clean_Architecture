# 🚀 .NET Core Microservice Starter Template

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Một template microservice .NET Core hoàn chỉnh với Clean Architecture, Domain-Driven Design, và đầy đủ infrastructure components.

## 📋 Tính năng chính

### 🏗️ Kiến trúc
- **Clean Architecture** với Domain-Driven Design
- **CQRS Pattern** với MediatR
- **Repository Pattern** với Unit of Work
- **Specification Pattern** cho queries phức tạp
- **Multi-tenancy** support
- **Domain Events** handling

### 🔐 Identity & Security
- JWT Authentication với refresh tokens
- Role-based & Claims-based authorization
- OpenTelemetry distributed tracing
- API versioning
- Swagger/OpenAPI documentation

### 🛠️ Infrastructure
- **PostgreSQL** - Database
- **Redis** - Distributed cache
- **RabbitMQ** - Message broker
- **MinIO** - Object storage (S3-compatible)
- **Elasticsearch** - Full-text search
- **Seq** - Centralized logging
- **Jaeger** - Distributed tracing
- **Nginx** - API Gateway/Reverse proxy

### 📊 Observability
- Health checks endpoints
- OpenTelemetry instrumentation
- Structured logging với Serilog
- Performance monitoring
- Distributed tracing

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (v2.0+)

### Khởi chạy với Docker (Khuyến nghị)

```bash
# 1. Clone repository
git clone <repository-url>
cd STC-Asp_DotNet_Core

# 2. Copy và cấu hình environment variables
cp .env.example .env

# 3. Deploy toàn bộ hệ thống
./scripts/deploy.sh
```

Hệ thống sẽ khởi động tất cả services, đợi khoảng 1-2 phút để các service khởi động hoàn tất.

### Truy cập ứng dụng

| Service | URL | Credentials |
|---------|-----|-------------|
| 📚 **API Documentation (Swagger)** | http://localhost/docs | - |
| 🔐 **Identity Service** | http://localhost:5001 | - |
| 🐘 **PgAdmin** | http://localhost:5050 | admin@admin.com / admin123 |
| 💾 **Redis Commander** | http://localhost:8083 | admin / admin123 |
| 🐰 **RabbitMQ Management** | http://localhost:15672 | admin / admin123 |
| 📦 **MinIO Console** | http://localhost:9001 | admin / admin123456 |
| 📊 **Seq Logs** | http://localhost:8084 | admin / admin123 |
| 🔍 **Jaeger Tracing** | http://localhost:16686 | - |

> ⚠️ **Production**: Thay đổi tất cả credentials mặc định trong file `.env`

## 📁 Cấu trúc dự án

```
STC-Asp_DotNet_Core/
├── src/                            # Source code
│   └── Identity/                   # Identity Service (Authentication)
│       ├── Api/                    # API Layer
│       ├── Application/            # Business logic
│       ├── Domain/                 # Domain entities
│       └── Infrastructure/         # Data access & external services
├── libs/                           # Shared libraries
│   ├── Api/                        # API base configuration
│   ├── Application/                # Application patterns (CQRS, etc)
│   ├── Contracts/                  # DTOs & API contracts
│   ├── Domain/                     # Domain base classes
│   ├── DynamicQuery/               # Dynamic query builder
│   ├── FluentConfiguration/        # Elasticsearch config
│   ├── Infrastructure/             # Data access infrastructure
│   ├── SharedKernel/               # Shared domain logic
│   └── Specification/              # Specification pattern
├── config/                         # Configuration files
│   └── otel-collector-config.yaml # OpenTelemetry config
├── scripts/                        # Deployment scripts
├── docker-compose.yml              # Docker orchestration
├── Dockerfile                      # .NET service image
└── nginx.conf                      # Nginx configuration
```

## 🛠️ Development

### Chạy local (không dùng Docker)

```bash
# 1. Khởi động infrastructure services
./scripts/start-all-infra.sh

# 2. Restore dependencies
dotnet restore

# 3. Run Identity Service
cd src/Identity/Api
dotnet run

# API sẽ chạy tại: http://localhost:5001
# Swagger UI: http://localhost:5001/docs
```

### Tạo migration mới

```bash
./scripts/create-migration.sh <MigrationName>
```

### Chạy tests

```bash
# Chạy tất cả tests
dotnet test

# Chạy với coverage
dotnet test /p:CollectCoverage=true
```

### Build Docker image

```bash
# Build image
docker build -t microservice-identity:latest .

# Run container
docker run -p 5001:8080 microservice-identity:latest
```

## 📚 Documentation

- [🚀 Docker Deployment Guide](docs/DOCKER_DEPLOYMENT.md) - Hướng dẫn deploy chi tiết
- [🏗️ Architecture](docs/ARCHITECTURE.md) - Kiến trúc hệ thống
- [🔐 Security](docs/SECURITY.md) - Bảo mật & authentication
- [📊 API Documentation](http://localhost/docs) - Swagger UI (sau khi chạy)

## 🔧 Các lệnh thường dùng

### Docker Commands

```bash
# Khởi động services
./scripts/deploy-start.sh

# Dừng services
./scripts/deploy-stop.sh

# Khởi động lại
./scripts/deploy-restart.sh

# Xem logs
./scripts/deploy-logs.sh [service-name]

# Kiểm tra trạng thái
./scripts/deploy-status.sh

# Dọn dẹp toàn bộ (⚠️ mất data)
./scripts/deploy-clean.sh
```

### Infrastructure Commands

```bash
# Khởi động infrastructure (Postgres, Redis, RabbitMQ, etc)
./scripts/start-all-infra.sh

# Dừng infrastructure
./scripts/stop-all-infra.sh

# Khởi động lại infrastructure
./scripts/restart-all-infra.sh
```

## 🏗️ Shared Libraries

Dự án sử dụng các shared libraries để tái sử dụng code:

- **Api**: Base API configuration, middleware, filters
- **Application**: CQRS patterns, behaviors, interfaces
- **Contracts**: DTOs, API contracts, constants
- **Domain**: Base entities, aggregate roots, converters
- **DynamicQuery**: Dynamic query builder cho filtering/sorting
- **FluentConfiguration**: Elasticsearch configuration
- **Infrastructure**: Repository, Unit of Work, data access
- **SharedKernel**: Common domain logic, exceptions, extensions
- **Specification**: Specification pattern implementation

## 📊 Technology Stack

### Backend
- .NET 8.0 (C# 12)
- ASP.NET Core Web API
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Serilog

### Database & Storage
- PostgreSQL 16
- Redis 7
- Elasticsearch 8
- MinIO (S3-compatible)

### Message Broker
- RabbitMQ 3.13

### Monitoring & Observability
- OpenTelemetry
- Jaeger
- Seq
- Health Checks

### DevOps
- Docker & Docker Compose
- Nginx
- Shell Scripts

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- .NET Community