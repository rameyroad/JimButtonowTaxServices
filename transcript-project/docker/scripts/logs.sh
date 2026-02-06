#!/bin/bash
# View logs for Transcript Analyzer services
#
# Usage:
#   ./logs.sh               # All services
#   ./logs.sh api           # API logs only
#   ./logs.sh web           # Web frontend logs only
#   ./logs.sh sqlserver     # SQL Server logs
#   ./logs.sh redis         # Redis logs
#   ./logs.sh seq           # Seq logs
#   ./logs.sh --follow      # Follow logs (all services)
#   ./logs.sh api --follow  # Follow specific service logs
#   ./logs.sh --tail 50     # Last 50 lines

set -e
cd "$(dirname "$0")/.."

SERVICE=""
EXTRA_ARGS=""

# Parse arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        api|web|sqlserver|redis|azurite|seq|adminer|redis-commander)
            SERVICE="transcript-$1"
            shift
            ;;
        --follow|-f)
            EXTRA_ARGS="$EXTRA_ARGS --follow"
            shift
            ;;
        --tail)
            EXTRA_ARGS="$EXTRA_ARGS --tail $2"
            shift 2
            ;;
        *)
            EXTRA_ARGS="$EXTRA_ARGS $1"
            shift
            ;;
    esac
done

# Determine which .env file to use
ENV_FILE=""
if [ -f .env ]; then
    ENV_FILE="--env-file .env"
fi

if [ -n "$SERVICE" ]; then
    echo "Showing logs for: $SERVICE"
    echo "================================================="
    docker logs $SERVICE $EXTRA_ARGS
else
    echo "Showing logs for all services"
    echo "================================================="
    docker compose $ENV_FILE --profile tools logs $EXTRA_ARGS
fi
