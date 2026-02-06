#!/bin/bash
# Check health status of Transcript Analyzer services
#
# Usage:
#   ./health-check.sh         # Check current status
#   ./health-check.sh --wait  # Wait for services to become healthy

set -e
cd "$(dirname "$0")/.."

# Load environment variables
if [ -f .env ]; then
    export $(grep -v '^#' .env | xargs)
fi

API_PORT="${API_PORT:-5000}"
WEB_PORT="${WEB_PORT:-3000}"
SEQ_UI_PORT="${SEQ_UI_PORT:-8081}"
REDIS_PORT="${REDIS_PORT:-6379}"
SQL_PORT="${SQL_PORT:-1433}"

WAIT_MODE=false
if [[ "$1" == "--wait" ]]; then
    WAIT_MODE=true
    MAX_WAIT=300  # 5 minutes
    WAITED=0
fi

check_container() {
    local name="$1"
    local status=$(docker inspect --format='{{.State.Status}}' "$name" 2>/dev/null || echo "not found")
    local health=$(docker inspect --format='{{.State.Health.Status}}' "$name" 2>/dev/null || echo "none")

    if [ "$status" = "running" ]; then
        if [ "$health" = "healthy" ]; then
            echo "  ✓ $name: running (healthy)"
            return 0
        elif [ "$health" = "unhealthy" ]; then
            echo "  ✗ $name: running (unhealthy)"
            return 1
        else
            echo "  ○ $name: running (no healthcheck)"
            return 0
        fi
    else
        echo "  ✗ $name: $status"
        return 1
    fi
}

check_http() {
    local name="$1"
    local url="$2"

    if curl -sf "$url" > /dev/null 2>&1; then
        echo "  ✓ $name: OK ($url)"
        return 0
    else
        echo "  ✗ $name: FAILED ($url)"
        return 1
    fi
}

check_tcp() {
    local name="$1"
    local host="$2"
    local port="$3"

    if nc -z "$host" "$port" 2>/dev/null; then
        echo "  ✓ $name: OK ($host:$port)"
        return 0
    else
        echo "  ✗ $name: FAILED ($host:$port)"
        return 1
    fi
}

run_checks() {
    local all_healthy=true

    echo ""
    echo "Container Status:"
    echo "-----------------"
    check_container "transcript-sqlserver" || all_healthy=false
    check_container "transcript-redis" || all_healthy=false
    check_container "transcript-azurite" || true  # No healthcheck
    check_container "transcript-seq" || true       # No healthcheck
    check_container "transcript-api" || true       # May not be running
    check_container "transcript-web" || true       # May not be running

    echo ""
    echo "Infrastructure Endpoints:"
    echo "-------------------------"
    check_tcp "SQL Server" "localhost" "$SQL_PORT" || all_healthy=false
    check_tcp "Redis" "localhost" "$REDIS_PORT" || all_healthy=false
    check_http "Seq UI" "http://localhost:$SEQ_UI_PORT" || all_healthy=false

    echo ""
    echo "Application Endpoints:"
    echo "----------------------"
    check_http "API Health" "http://localhost:$API_PORT/health" || true
    check_http "Web Frontend" "http://localhost:$WEB_PORT" || true

    if $all_healthy; then
        echo ""
        echo "================================================="
        echo "Infrastructure services are healthy!"
        echo "================================================="
        return 0
    else
        return 1
    fi
}

echo "================================================="
echo "Transcript Analyzer - Health Check"
echo "================================================="

if $WAIT_MODE; then
    echo "Waiting for services to become healthy (max ${MAX_WAIT}s)..."

    while ! run_checks 2>/dev/null; do
        WAITED=$((WAITED + 5))
        if [ $WAITED -ge $MAX_WAIT ]; then
            echo ""
            echo "Timeout waiting for services to become healthy"
            exit 1
        fi
        echo ""
        echo "Waiting... ($WAITED/${MAX_WAIT}s)"
        sleep 5
    done
else
    run_checks
fi
