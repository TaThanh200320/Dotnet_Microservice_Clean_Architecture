#!/bin/bash

set -e

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# ASCII Art Banner
print_banner() {
    echo ""
    echo -e "${BLUE}╔══════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BLUE}║                                                              ║${NC}"
    echo -e "${BLUE}║        Microservice Deployment Script                       ║${NC}"
    echo -e "${BLUE}║        .NET Core Microservice Platform                      ║${NC}"
    echo -e "${BLUE}║                                                              ║${NC}"
    echo -e "${BLUE}╚══════════════════════════════════════════════════════════════╝${NC}"
    echo ""
}

print_banner

print_step "🚀 Starting deployment process..."

# Check if Docker is installed
if ! command_exists docker; then
    print_error "Docker is not installed. Please install Docker first."
    print_info "Visit: https://docs.docker.com/get-docker/"
    exit 1
fi

print_info "✓ Docker is installed"

# Check if Docker Compose is installed
if ! docker compose version >/dev/null 2>&1; then
    print_error "Docker Compose (v2) is not installed. Please install Docker Compose v2 first."
    print_info "Visit: https://docs.docker.com/compose/install/"
    exit 1
fi

print_info "✓ Docker Compose v2 is installed"

# Check if Docker daemon is running
if ! docker info >/dev/null 2>&1; then
    print_error "Docker daemon is not running. Please start Docker first."
    exit 1
fi

print_info "✓ Docker daemon is running"

# Load environment variables
if [ -f .env ]; then
    print_info "Loading environment variables from .env file..."
    export $(cat .env | grep -v '^#' | grep -v '^$' | xargs)
else
    print_warning ".env file not found. Using default values."
    print_info "You can create .env file from .env.example for custom configuration."
fi

# Generate JWT keys if script exists and keys don't exist
if [ -f "scripts/generate-jwt-keys.sh" ] && [ ! -f "src/Identity/Api/certs/private.key" ]; then
    print_step "Generating JWT keys..."
    chmod +x scripts/generate-jwt-keys.sh
    ./scripts/generate-jwt-keys.sh
fi

# Create necessary directories
print_step "Creating necessary directories..."
mkdir -p nginx-logs
mkdir -p src/Identity/Api/certs

# Stop and remove existing containers
print_step "Stopping and removing existing containers..."
docker compose down -v 2>/dev/null || true

# Clean up dangling images
print_step "Cleaning up old Docker resources..."
docker system prune -f

# Build and start infrastructure services first
print_step "Starting infrastructure services (Database, Cache, Message Queue, etc.)..."
docker compose up -d postgres pgadmin adminer redis redis-commander rabbitmq minio elasticsearch seq jaeger otel-collector

# Wait for infrastructure to be ready
print_step "Waiting for infrastructure services to be healthy..."
print_info "This may take a minute or two..."

# Function to wait for service
wait_for_service() {
    local service=$1
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if docker compose ps | grep -q "$service.*healthy\|Up"; then
            print_info "✓ $service is ready"
            return 0
        fi
        echo -n "."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    print_warning "$service might not be fully ready, continuing anyway..."
    return 1
}

wait_for_service "postgres"
wait_for_service "redis"
wait_for_service "rabbitmq"
wait_for_service "elasticsearch"

echo ""

# Build application
print_step "Building application services..."
docker compose build --no-cache identity-service

# Start application services
print_step "Starting application services..."
docker compose up -d identity-service nginx

# Wait a bit for services to start
sleep 10

# Check service health
print_step "Checking service health..."
docker compose ps

echo ""
echo -e "${GREEN}╔══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║                                                              ║${NC}"
echo -e "${GREEN}║              ✅ Deployment Completed Successfully!           ║${NC}"
echo -e "${GREEN}║                                                              ║${NC}"
echo -e "${GREEN}╚══════════════════════════════════════════════════════════════╝${NC}"
echo ""

print_info "🌐 Services are running at:"
echo ""
echo -e "  ${BLUE}API Gateway (Nginx):${NC}         http://localhost${NGINX_HTTP_PORT:+:$NGINX_HTTP_PORT}"
echo -e "  ${BLUE}Identity Service:${NC}            http://localhost:${IDENTITY_SERVICE_PORT:-5001}"
echo ""
echo -e "  ${BLUE}Database Management:${NC}"
echo -e "    - PgAdmin:                     http://localhost:${PGADMIN_PORT:-5050}"
echo -e "    - Adminer:                     http://localhost:${ADMINER_PORT:-8082}"
echo ""
echo -e "  ${BLUE}Monitoring & Observability:${NC}"
echo -e "    - Redis Commander:             http://localhost:${REDIS_COMMANDER_PORT:-8083}"
echo -e "    - RabbitMQ Management:         http://localhost:${RABBITMQ_MANAGEMENT_PORT:-15672}"
echo -e "    - MinIO Console:               http://localhost:${MINIO_CONSOLE_PORT:-9001}"
echo -e "    - Seq Logs:                    http://localhost:${SEQ_UI_PORT:-8084}"
echo -e "    - Jaeger Tracing:              http://localhost:${JAEGER_UI_PORT:-16686}"
echo -e "    - Elasticsearch:               http://localhost:${ELASTICSEARCH_PORT:-9200}"
echo ""

print_info "📋 Default credentials:"
echo -e "  ${YELLOW}PostgreSQL:${NC}     admin / admin123"
echo -e "  ${YELLOW}PgAdmin:${NC}        admin@admin.com / admin123"
echo -e "  ${YELLOW}RabbitMQ:${NC}       admin / admin123"
echo -e "  ${YELLOW}Redis Commander:${NC} admin / admin123"
echo -e "  ${YELLOW}MinIO:${NC}          admin / admin123456"
echo ""

print_info "📝 Useful commands:"
echo -e "  ${BLUE}View logs:${NC}           ./scripts/deploy-logs.sh"
echo -e "  ${BLUE}Stop services:${NC}       ./scripts/deploy-stop.sh"
echo -e "  ${BLUE}Restart services:${NC}    ./scripts/deploy-restart.sh"
echo -e "  ${BLUE}Clean up:${NC}            ./scripts/deploy-clean.sh"
echo ""

# Ask if user wants to see logs
read -p "Do you want to view the logs? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    print_info "Showing logs (Press Ctrl+C to exit)..."
    docker compose logs -f
fi
