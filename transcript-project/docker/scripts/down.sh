#!/bin/bash
# Stop Transcript Analyzer services
#
# Usage:
#   ./down.sh           # Stop services (keep volumes)
#   ./down.sh -v        # Stop and remove volumes (DELETE DATA!)
#   ./down.sh --rmi all # Stop and remove images

set -e
cd "$(dirname "$0")/.."

echo "================================================="
echo "Transcript Analyzer - Stopping Services"
echo "================================================="

# Determine which .env file to use
ENV_FILE=""
if [ -f .env ]; then
    ENV_FILE="--env-file .env"
fi

# Stop Tilt if it's running
if command -v tilt &> /dev/null; then
    echo "Stopping Tilt (if running)..."
    tilt down 2>/dev/null || true
fi

# Stop all profiles (use tilt-compose.yml if it exists for consistency)
if [ -f tilt-compose.yml ]; then
    docker compose -f tilt-compose.yml $ENV_FILE down "$@" 2>/dev/null || true
fi

# Stop all profiles
docker compose $ENV_FILE --profile tools down "$@"

# Clean up orphaned containers
docker compose down --remove-orphans 2>/dev/null || true

echo ""
echo "Services stopped."

# Show volume warning if -v was used
if [[ "$*" == *"-v"* ]]; then
    echo ""
    echo "================================================="
    echo "WARNING: Volumes were removed. All data deleted:"
    echo "  - SQL Server database"
    echo "  - Redis cache"
    echo "  - Azurite storage"
    echo "  - Seq logs"
    echo "================================================="
fi
