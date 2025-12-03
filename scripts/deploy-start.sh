#!/bin/bash

# Colors
GREEN='\033[0;32m'
NC='\033[0m'

echo -e "${GREEN}🚀 Starting all services...${NC}"
echo ""

docker compose up -d

echo ""
echo -e "${GREEN}✅ All services started!${NC}"
echo ""

docker compose ps
