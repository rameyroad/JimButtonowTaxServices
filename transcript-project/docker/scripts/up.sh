#!/bin/bash
# Start Transcript Analyzer services
#
# Usage:
#   ./up.sh              # Start infrastructure only (SQL Server, Redis, Azurite, Seq)
#   ./up.sh api          # Start API + infrastructure
#   ./up.sh full         # Start all services
#   ./up.sh tools        # Start all + dev tools (Adminer, Redis Commander)
#   ./up.sh --build      # Force rebuild images
#   ./up.sh --follow     # Follow logs after starting

set -e
cd "$(dirname "$0")/.."

PROFILE="${1:-}"
shift 2>/dev/null || true

# Check if first arg is a flag
if [[ "$PROFILE" == --* ]]; then
    EXTRA_ARGS="$PROFILE $*"
    PROFILE=""
else
    EXTRA_ARGS="$*"
fi

# Map profile to docker compose profiles
case "$PROFILE" in
    "")
        COMPOSE_PROFILES=""
        PROFILE_DESC="infrastructure only (SQL Server, Redis, Azurite, Seq)"
        ;;
    api|backend)
        COMPOSE_PROFILES="api"
        PROFILE_DESC="API + infrastructure"
        ;;
    web|frontend)
        COMPOSE_PROFILES="web"
        PROFILE_DESC="Web + API + infrastructure"
        ;;
    full)
        COMPOSE_PROFILES="full"
        PROFILE_DESC="full stack (API + Web + infrastructure)"
        ;;
    tools|all)
        COMPOSE_PROFILES="tools"
        PROFILE_DESC="full stack + dev tools (Adminer, Redis Commander)"
        ;;
    *)
        echo "Unknown profile: $PROFILE"
        echo ""
        echo "Available profiles:"
        echo "  (none)   - Infrastructure only (SQL Server, Redis, Azurite, Seq)"
        echo "  api      - API + infrastructure"
        echo "  full     - Full stack (API + Web + infrastructure)"
        echo "  tools    - Full stack + dev tools"
        echo ""
        echo "Options:"
        echo "  --build   - Force rebuild images"
        echo "  --follow  - Follow logs after starting"
        exit 1
        ;;
esac

echo "================================================="
echo "Transcript Analyzer - Starting Services"
echo "================================================="
echo "Profile: $PROFILE_DESC"
echo ""

# Determine which .env file to use (prefer local .env, fallback to template warning)
ENV_FILE=""
if [ -f .env ]; then
    ENV_FILE="--env-file .env"
    echo "Using environment from: .env"
else
    echo "Warning: No .env file found!"
    echo "Copy .env.template to .env: cp .env.template .env"
    echo "Using default values from docker-compose.yml"
    echo ""
fi

# Determine follow flag
FOLLOW=""
DETACH="-d"
for arg in $EXTRA_ARGS; do
    if [[ "$arg" == "--follow" ]] || [[ "$arg" == "-f" ]]; then
        FOLLOW="yes"
        DETACH=""
        EXTRA_ARGS="${EXTRA_ARGS/--follow/}"
        EXTRA_ARGS="${EXTRA_ARGS/-f/}"
    fi
done

# Start services
if [ -n "$COMPOSE_PROFILES" ]; then
    docker compose $ENV_FILE --profile "$COMPOSE_PROFILES" up $DETACH $EXTRA_ARGS
else
    docker compose $ENV_FILE up $DETACH $EXTRA_ARGS
fi

if [ -z "$FOLLOW" ] && [ -n "$DETACH" ]; then
    echo ""
    echo "================================================="
    echo "Services started in background"
    echo "================================================="
    echo ""
    echo "Commands:"
    echo "  ./scripts/logs.sh         - View logs"
    echo "  ./scripts/health-check.sh - Check service health"
    echo "  ./scripts/down.sh         - Stop services"
    echo ""
    echo "Service URLs:"
    echo "  API:          http://localhost:${API_PORT:-5000}"
    echo "  API Health:   http://localhost:${API_PORT:-5000}/health"
    echo "  Web:          http://localhost:${WEB_PORT:-3000}"
    echo "  Seq (Logs):   http://localhost:${SEQ_UI_PORT:-8081}"

    if [[ "$COMPOSE_PROFILES" == "tools" ]]; then
        echo "  Adminer:      http://localhost:${AZURE_DATA_STUDIO_PORT:-5050}"
        echo "  Redis Cmd:    http://localhost:${REDIS_COMMANDER_PORT:-8082}"
    fi
    echo ""
fi
