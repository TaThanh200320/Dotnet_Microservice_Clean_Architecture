#!/bin/bash

# Colors
RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
NC='\033[0m'

echo -e "${YELLOW}⚠️  WARNING: This will remove all containers, volumes, and networks!${NC}"
echo -e "${RED}All data will be lost!${NC}"
echo ""
read -p "Are you sure you want to continue? (yes/no) " -r
echo

if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo "Aborted."
    exit 0
fi

echo -e "${RED}🧹 Cleaning up Docker resources...${NC}"
echo ""

# Stop and remove all containers, volumes, and networks
docker compose down -v --remove-orphans

# Remove unused Docker resources
docker system prune -f --volumes

echo ""
echo -e "${GREEN}✅ Cleanup completed!${NC}"
