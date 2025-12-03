#!/bin/bash

# Colors
BLUE='\033[0;34m'
GREEN='\033[0;32m'
NC='\033[0m'

echo -e "${BLUE}🔄 Restarting all services...${NC}"
echo ""

docker compose restart

echo ""
echo -e "${GREEN}✅ All services restarted!${NC}"
echo ""

docker compose ps
