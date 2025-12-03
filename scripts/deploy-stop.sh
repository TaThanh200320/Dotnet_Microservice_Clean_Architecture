#!/bin/bash

# Colors
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
NC='\033[0m'

echo -e "${YELLOW}🛑 Stopping all services...${NC}"
echo ""

docker compose stop

echo ""
echo -e "${GREEN}✅ All services stopped!${NC}"
