# 🚀 Quick Start Guide

Hướng dẫn nhanh để khởi chạy hệ thống microservice .NET Core.

## Yêu cầu

- Docker 20.10+
- Docker Compose v2.0+
- RAM: 8GB minimum
- Disk: 20GB free space

## Bước 1: Clone Repository

```bash
git clone <repository-url>
cd Dotnet_Microservice_Clean_Architecture
```

## Bước 2: Cấu hình Environment

```bash
# Copy template
cp .env.example .env

# Chỉnh sửa file .env nếu cần
nano .env
```

## Bước 3: Deploy

```bash
# Khởi động toàn bộ hệ thống
./scripts/deploy.sh
```

Đợi khoảng 1-2 phút để các service khởi động.

## Bước 4: Kiểm tra

```bash
# Kiểm tra trạng thái
./scripts/deploy-status.sh

# Xem logs
./scripts/deploy-logs.sh
```

## Bước 5: Truy cập

| Service | URL |
|---------|-----|
| API Docs | http://localhost/docs |
| Identity API | http://localhost:5001 |
| PgAdmin | http://localhost:5050 |
| RabbitMQ | http://localhost:15672 |
| MinIO | http://localhost:9001 |
| Seq Logs | http://localhost:8084 |
| Jaeger | http://localhost:16686 |

## Credentials mặc định

- PostgreSQL: `admin` / `admin123`
- PgAdmin: `admin@admin.com` / `admin123`
- RabbitMQ: `admin` / `admin123`
- MinIO: `admin` / `admin123456`
- Seq: `admin` / `admin123`
- Redis Commander: `admin` / `admin123`

## API Testing

### 1. Swagger UI

Mở http://localhost/docs trong browser để xem API documentation.

### 2. Create User

```bash
curl -X POST http://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### 3. Login

```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!"
  }'
```

### 4. Access Protected Endpoint

```bash
curl -X GET http://localhost:5001/api/v1/users/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## Các lệnh hữu ích

```bash
# Khởi động services
./scripts/deploy-start.sh

# Dừng services (giữ data)
./scripts/deploy-stop.sh

# Khởi động lại
./scripts/deploy-restart.sh

# Xem logs của service cụ thể
./scripts/deploy-logs.sh identity-service

# Dọn dẹp hoàn toàn (⚠️ mất data)
./scripts/deploy-clean.sh
```

## Troubleshooting

### Port đã được sử dụng

```bash
# Kiểm tra port
sudo lsof -i :5001

# Hoặc thay đổi port trong .env
echo "IDENTITY_SERVICE_PORT=5002" >> .env
```

### Database connection error

```bash
# Kiểm tra PostgreSQL logs
./scripts/deploy-logs.sh postgres

# Restart PostgreSQL
docker compose restart postgres
```

### Service không start

```bash
# Kiểm tra logs
./scripts/deploy-logs.sh <service-name>

# Rebuild image
docker compose build <service-name>

# Restart service
docker compose up -d <service-name>
```

## Tài liệu đầy đủ

- [📚 README](../README.md) - Tổng quan dự án
- [🚀 Docker Deployment](DOCKER_DEPLOYMENT.md) - Hướng dẫn chi tiết
- [🏗️ Architecture](ARCHITECTURE.md) - Kiến trúc hệ thống
- [🔐 Security](SECURITY.md) - Bảo mật & Authentication

## Hỗ trợ

Nếu gặp vấn đề:

1. Kiểm tra logs: `./scripts/deploy-logs.sh`
2. Kiểm tra status: `./scripts/deploy-status.sh`
3. Xem documentation: [docs/](.)
