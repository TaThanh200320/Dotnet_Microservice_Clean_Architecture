#!/bin/bash

# Colors
BLUE='\033[0;34m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Show logs for all services or specific service
if [ -z "$1" ]; then
    echo -e "${BLUE}📋 Showing logs for all services...${NC}"
    echo -e "${YELLOW}Press Ctrl+C to exit${NC}"
    echo ""
    docker compose logs -f --tail=100
else
    echo -e "${BLUE}📋 Showing logs for $1...${NC}"
    echo -e "${YELLOW}Press Ctrl+C to exit${NC}"
    echo ""
    docker compose logs -f --tail=100 "$1"
fi
