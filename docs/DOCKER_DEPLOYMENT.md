# 🚀 Hướng dẫn Deploy với Docker

## 📋 Tổng quan

Hệ thống microservice .NET Core với các thành phần:
- **Identity Service**: Quản lý xác thực và phân quyền
- **PostgreSQL**: Database
- **Redis**: Cache
- **RabbitMQ**: Message broker
- **MinIO**: Object storage (S3-compatible)
- **Elasticsearch**: Search engine
- **Seq**: Centralized logging
- **OpenTelemetry**: Observability
- **Nginx**: Reverse proxy

## 🛠️ Yêu cầu hệ thống

- Docker 20.10+ (với Docker Compose V2 plugin)
- Docker Compose v2.0+
- RAM tối thiểu: 8GB
- Disk space: 20GB

## 📦 Các file đã tạo

```
├── Dockerfile                      # Build image cho .NET services
├── docker compose.yml              # Orchestration cho toàn bộ hệ thống
├── .env.example                    # Template cho environment variables
├── .dockerignore                   # Exclude files khi build
└── scripts/
    ├── deploy.sh                   # Script deploy chính
    ├── deploy-start.sh             # Khởi động services
    ├── deploy-stop.sh              # Dừng services
    ├── deploy-restart.sh           # Khởi động lại services
    ├── deploy-logs.sh              # Xem logs
    ├── deploy-status.sh            # Kiểm tra trạng thái
    └── deploy-clean.sh             # Xóa toàn bộ (cẩn thận!)
```

## 🚀 Cách sử dụng

### Lần đầu tiên (Deploy mới)

```bash
# 1. Copy file environment và chỉnh sửa nếu cần
cp .env.example .env

# 2. Chỉnh sửa các biến môi trường trong file .env
nano .env

# 3. Deploy toàn bộ hệ thống
./scripts/deploy.sh
```

### Các lệnh thường dùng

```bash
# Khởi động services
./scripts/deploy-start.sh

# Dừng services (giữ lại data)
./scripts/deploy-stop.sh

# Khởi động lại services
./scripts/deploy-restart.sh

# Xem logs tất cả services
./scripts/deploy-logs.sh

# Xem logs của service cụ thể
./scripts/deploy-logs.sh identity-service
./scripts/deploy-logs.sh postgres

# Kiểm tra trạng thái và tài nguyên
./scripts/deploy-status.sh

# Xóa toàn bộ (bao gồm data) - CẨN THẬN!
./scripts/deploy-clean.sh
```

## 🌐 Truy cập các services

Sau khi deploy thành công, các services sẽ chạy tại:

| Service | URL | Mô tả |
|---------|-----|-------|
| API Gateway | http://localhost | Nginx reverse proxy |
| Identity Service | http://localhost:5001 | Authentication API |
| **PgAdmin** | http://localhost:5050 | PostgreSQL Web UI |
| **Adminer** | http://localhost:8082 | Database management |
| **Redis Commander** | http://localhost:8083 | Redis Web UI |
| RabbitMQ Management | http://localhost:15672 | Message queue UI |
| MinIO Console | http://localhost:9001 | Object storage UI |
| Seq Logs | http://localhost:8084 | Centralized logs |
| **Jaeger UI** | http://localhost:16686 | Distributed tracing |
| Elasticsearch | http://localhost:9200 | Search API |

## 🔐 Credentials mặc định

**PostgreSQL:**

- User: `admin`
- Password: `admin123`
- Database: `identitydb`

**PgAdmin:**

- Email: `admin@admin.com`
- Password: `admin123`

**RabbitMQ:**

- User: `admin`
- Password: `admin123`

**MinIO:**

- User: `admin`
- Password: `admin123456`

**Seq:**

- Username: `admin`
- Password: `admin123`

**Redis:**

- Password: `redis123`

**Redis Commander:**

- User: `admin`
- Password: `admin123`

> ⚠️ **Lưu ý**: Thay đổi các credentials này trong file `.env` cho môi trường production!

## 🔧 Cấu hình nâng cao

### Thay đổi port

Chỉnh sửa file `.env`:

```bash
IDENTITY_SERVICE_PORT=5001
NGINX_HTTP_PORT=80
POSTGRES_PORT=5432
# ... các port khác
```

### Thay đổi JWT configuration

```bash
JWT_SECRET_KEY=your-secret-key-here
JWT_ISSUER=your-issuer
JWT_AUDIENCE=your-audience
JWT_EXPIRATION_MINUTES=60
```

### Scale services

```bash
# Scale Identity service lên 3 instances
docker compose up -d --scale identity-service=3
```

## 📊 Monitoring

### Xem logs realtime

```bash
# Tất cả services
docker compose logs -f

# Service cụ thể
docker compose logs -f identity-service
docker compose logs -f postgres
```

### Kiểm tra resource usage

```bash
./scripts/deploy-status.sh
```

hoặc

```bash
docker stats
```

## 🐛 Troubleshooting

### Service không start được

```bash
# Kiểm tra logs
./scripts/deploy-logs.sh <service-name>

# Kiểm tra trạng thái
docker compose ps

# Restart service cụ thể
docker compose restart <service-name>
```

### Database connection error

```bash
# Kiểm tra PostgreSQL đã ready chưa
docker compose logs postgres

# Restart PostgreSQL
docker compose restart postgres

# Kiểm tra connection string trong .env
cat .env | grep POSTGRES
```

### Port đã được sử dụng

```bash
# Tìm process đang dùng port
sudo lsof -i :5001

# Hoặc thay đổi port trong .env
echo "IDENTITY_SERVICE_PORT=5002" >> .env
```

### Hết dung lượng disk

```bash
# Clean up unused Docker resources
docker system prune -a --volumes

# Xem dung lượng đang dùng
docker system df
```

## 🔄 Update và Rebuild

### Rebuild service sau khi thay đổi code

```bash
# Rebuild Identity service
docker compose build identity-service

# Restart với image mới
docker compose up -d identity-service
```

### Update toàn bộ

```bash
# Stop services
./scripts/deploy-stop.sh

# Pull latest images
docker compose pull

# Rebuild custom images
docker compose build --no-cache

# Start với images mới
./scripts/deploy-start.sh
```

## 📝 Backup và Restore

### Backup database

```bash
# Backup PostgreSQL
docker exec microservice-postgres pg_dump -U admin identitydb > backup_$(date +%Y%m%d).sql

# Backup tất cả volumes
docker run --rm -v microservice-postgres-data:/data -v $(pwd):/backup \
  alpine tar czf /backup/postgres-backup.tar.gz /data
```

### Restore database

```bash
# Restore PostgreSQL
cat backup_20241128.sql | docker exec -i microservice-postgres psql -U admin identitydb
```

## 🚨 Production Checklist

Trước khi deploy lên production, đảm bảo:

- [ ] Đã thay đổi tất cả default passwords
- [ ] Đã cấu hình HTTPS/SSL certificates
- [ ] Đã setup backup tự động
- [ ] Đã cấu hình monitoring và alerting
- [ ] Đã test disaster recovery
- [ ] Đã review security settings
- [ ] Đã cấu hình resource limits
- [ ] Đã setup log rotation

## 📚 Tài liệu thêm

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)

## 💬 Hỗ trợ

Nếu gặp vấn đề, kiểm tra:
1. Logs: `./scripts/deploy-logs.sh`
2. Status: `./scripts/deploy-status.sh`
3. Docker: `docker compose ps`
