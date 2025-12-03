#!/bin/bash

# Script to start all Docker Compose services
# Usage: ./start-all-services.sh

set -e

echo "Starting all Docker Compose services..."

# Define the base directory
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCKERS_DIR="$BASE_DIR/dockers"

# Array of service directories
services=(
    "Postgresql"
    "Redis"
    "ElasticSearch"
    "MinioS3"
    "Seq"
    "Otel"
)

# Start each service
for service in "${services[@]}"; do
    service_path="$DOCKERS_DIR/$service"
    
    if [ -d "$service_path" ]; then
        echo ""
        echo "===================================="
        echo "Starting $service..."
        echo "===================================="
        cd "$service_path"
        docker compose up -d
        echo "✓ $service started successfully"
    else
        echo "⚠ Warning: $service directory not found at $service_path"
    fi
done

echo ""
echo "===================================="
echo "All services started successfully!"
echo "===================================="
echo ""
echo "To view running containers, run: docker ps"
echo "To stop all services, run: ./stop-all-services.sh"
