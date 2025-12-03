#!/bin/bash

# Script to stop all Docker Compose services
# Usage: ./stop-all-services.sh

set -e

echo "Stopping all Docker Compose services..."

# Define the base directory
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCKERS_DIR="$BASE_DIR/dockers"

# Array of service directories
services=(
    "Otel"
    "Seq"
    "MinioS3"
    "ElasticSearch"
    "Redis"
    "Postgresql"
)

# Stop each service (in reverse order)
for service in "${services[@]}"; do
    service_path="$DOCKERS_DIR/$service"
    
    if [ -d "$service_path" ]; then
        echo ""
        echo "===================================="
        echo "Stopping $service..."
        echo "===================================="
        cd "$service_path"
        docker compose down
        echo "✓ $service stopped successfully"
    else
        echo "⚠ Warning: $service directory not found at $service_path"
    fi
done

echo ""
echo "===================================="
echo "All services stopped successfully!"
echo "===================================="
