#!/bin/bash
# Start Transcript Analyzer with Tilt hot-reload
#
# Usage:
#   ./tilt-up.sh           # Start with full profile (default)
#   ./tilt-up.sh api       # API profile only
#   ./tilt-up.sh infra     # Infrastructure only
#   ./tilt-up.sh tools     # Full stack + dev tools
#
# Profiles:
#   full    - All services (default)
#   api     - API + infrastructure
#   infra   - Infrastructure only
#   tools   - All services + dev tools

set -e
cd "$(dirname "$0")/.."

PROFILE="${1:-full}"

# Validate profile
case "$PROFILE" in
    full|api|web|infra|tools)
        ;;
    *)
        echo "Unknown profile: $PROFILE"
        echo ""
        echo "Available profiles:"
        echo "  full   - All services (default)"
        echo "  api    - API + infrastructure (run frontend from IDE)"
        echo "  infra  - Infrastructure only (run API/frontend from IDE)"
        echo "  tools  - Full stack + dev tools"
        exit 1
        ;;
esac

# Check if Tilt is installed
if ! command -v tilt &> /dev/null; then
    echo "Error: Tilt is not installed"
    echo ""
    echo "Install Tilt:"
    echo "  macOS:   brew install tilt-dev/tap/tilt"
    echo "  Linux:   curl -fsSL https://raw.githubusercontent.com/tilt-dev/tilt/master/scripts/install.sh | bash"
    echo "  Windows: scoop install tilt"
    echo ""
    echo "Or visit: https://docs.tilt.dev/install.html"
    exit 1
fi

# Copy .env if it doesn't exist
if [ ! -f .env ] && [ -f .env.template ]; then
    echo "Creating .env from template..."
    cp .env.template .env
fi

echo "================================================="
echo "Transcript Analyzer - Starting with Tilt"
echo "================================================="
echo "Profile: $PROFILE"
echo ""
echo "Tilt UI will be available at: http://localhost:10350"
echo ""

# Start Tilt with the selected profile
tilt up -- "$PROFILE"
