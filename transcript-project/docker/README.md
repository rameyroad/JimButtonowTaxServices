# Transcript Analyzer - Docker Development Environment

Local development environment with hot-reload support using Docker Compose and Tilt.

## Quick Start

```bash
# Copy environment template
cp .env.template .env

# Start infrastructure only (SQL Server, Redis, Azurite, Seq)
./scripts/up.sh

# Or start with API
./scripts/up.sh api

# Or start full stack
./scripts/up.sh full

# Or use Tilt for hot-reload development
./scripts/tilt-up.sh
```

## Services

| Service | Description | Default Port |
|---------|-------------|--------------|
| `sqlserver` | SQL Server 2022 database | 1433 |
| `redis` | Redis 7 cache | 6379 |
| `azurite` | Azure Storage emulator | 10000-10002 |
| `seq` | Centralized logging | 5341 (ingestion), 8081 (UI) |
| `api` | .NET 9 Backend API | 5000 |
| `web` | Next.js Frontend | 3000 |
| `adminer` | Database admin UI (optional) | 5050 |
| `redis-commander` | Redis UI (optional) | 8082 |

## Profiles

Start only the services you need:

| Profile | Services | Use Case |
|---------|----------|----------|
| (none) | Infrastructure only | Run API/web from IDE with debugging |
| `api` | API + infrastructure | Run web from IDE |
| `full` | All services | Full stack development |
| `tools` | All + dev tools | Full stack + Adminer, Redis Commander |

```bash
# Examples
./scripts/up.sh           # Infrastructure only (default)
./scripts/up.sh api       # API + infrastructure
./scripts/up.sh full      # Full stack
./scripts/up.sh tools     # Full stack + dev tools
./scripts/tilt-up.sh api  # API with Tilt hot-reload
```

## Scripts

| Script | Description |
|--------|-------------|
| `up.sh` | Start services with profile selection |
| `down.sh` | Stop services and cleanup |
| `logs.sh` | View logs with service shortcuts |
| `health-check.sh` | Check service health status |
| `tilt-up.sh` | Start with Tilt hot-reload |

### Script Examples

```bash
# Start services
./scripts/up.sh                 # Infrastructure only, detached
./scripts/up.sh api --follow    # API, follow logs
./scripts/up.sh full --build    # Force rebuild images

# Stop services
./scripts/down.sh               # Keep volumes
./scripts/down.sh -v            # Remove volumes (DELETE DATA!)

# View logs
./scripts/logs.sh               # All services
./scripts/logs.sh api           # API logs only
./scripts/logs.sh --tail 50     # Last 50 lines
./scripts/logs.sh api --follow  # Follow API logs

# Health check
./scripts/health-check.sh       # Check status
./scripts/health-check.sh --wait # Wait for healthy
```

## Configuration

Copy `.env.template` to `.env` and customize:

```bash
# Ports
API_PORT=5000
WEB_PORT=3000
SQL_PORT=1433
REDIS_PORT=6379
SEQ_UI_PORT=8081

# Database
SA_PASSWORD=YourStrong@Passw0rd

# Authentication
USE_DEV_AUTH=true  # Set to false for Auth0

# Platform Admin (seeded on startup)
PLATFORM_ADMIN_EMAIL=admin@local.dev
```

## Development Authentication

For local development without Auth0, set `USE_DEV_AUTH=true` in your `.env` file. This enables a simple dev auth mode that bypasses Auth0.

When ready to test with Auth0:
1. Set `USE_DEV_AUTH=false`
2. Configure the Auth0 settings in `.env`

## Service URLs

| Service | URL |
|---------|-----|
| API | http://localhost:5000 |
| API Health | http://localhost:5000/health |
| Web | http://localhost:3000 |
| Seq UI | http://localhost:8081 |
| Adminer | http://localhost:5050 |
| Redis Commander | http://localhost:8082 |

## Tilt Usage

Tilt provides a web UI and automatic hot-reload:

```bash
./scripts/tilt-up.sh           # Start Tilt (full profile)
./scripts/tilt-up.sh api       # API profile only
./scripts/tilt-up.sh infra     # Infrastructure only
```

Access the Tilt UI at http://localhost:10350

## Development Workflows

### Backend Developer (Debug API in IDE)

```bash
# Start infrastructure
./scripts/up.sh

# Open backend in IDE (VS Code, Rider, Visual Studio)
# Run/debug the API project
# API connects to containerized SQL Server, Redis, Azurite
```

### Frontend Developer (Debug Frontend in IDE)

```bash
# Start infrastructure + API
./scripts/up.sh api

# In another terminal
cd ../frontend
npm run dev
```

### Full Stack Development

```bash
# Start everything with hot-reload
./scripts/up.sh full

# Or use Tilt for enhanced experience
./scripts/tilt-up.sh
```

## Troubleshooting

### Container won't start

```bash
# Check container logs
docker logs transcript-api

# Force rebuild
./scripts/down.sh
./scripts/up.sh --build
```

### Port conflicts

Edit `.env` to change default ports:
```bash
API_PORT=5001
WEB_PORT=3001
```

### SQL Server connection issues

```bash
# Check if SQL Server is healthy
docker logs transcript-sqlserver

# Verify connectivity
docker exec -it transcript-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" -C
```

### Redis connection issues

```bash
# Check if Redis is healthy
docker logs transcript-redis

# Connect manually
docker exec -it transcript-redis redis-cli ping
```

### API startup issues

```bash
# Check API logs
./scripts/logs.sh api

# Verify database connection
./scripts/health-check.sh
```

### Hot-reload not working

Ensure polling is enabled (should be automatic):
- API: `DOTNET_USE_POLLING_FILE_WATCHER=1`
- Frontend: `WATCHPACK_POLLING=true`

### Clear all data

```bash
./scripts/down.sh -v    # WARNING: Deletes all volumes
```

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Docker Network                            │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌───────────┐  ┌───────┐  ┌─────────┐  ┌─────┐            │
│  │ sqlserver │  │ redis │  │ azurite │  │ seq │            │
│  │  :1433    │  │ :6379 │  │ :10000+ │  │:8081│            │
│  └─────┬─────┘  └───┬───┘  └────┬────┘  └──┬──┘            │
│        │            │           │          │                │
│        └────────────┴───────────┴──────────┘                │
│                         │                                    │
│                    ┌────┴────┐                              │
│                    │   api   │ (logs to seq)                │
│                    │  :5000  │                              │
│                    └────┬────┘                              │
│                         │                                    │
│                    ┌────┴────┐                              │
│                    │   web   │                              │
│                    │  :3000  │                              │
│                    └─────────┘                              │
│                                                              │
│  Optional (tools profile):                                   │
│  ┌─────────┐   ┌─────────────────┐                         │
│  │ adminer │   │ redis-commander │                         │
│  │  :5050  │   │     :8082       │                         │
│  └─────────┘   └─────────────────┘                         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## Requirements

- Docker Desktop 4.x (or Docker Engine + Compose V2)
- 8GB+ RAM recommended
- ~5GB disk space for images

### Optional

- Tilt 0.33+ (for enhanced hot-reload)
- .NET 9 SDK (if running API from IDE)
- Node.js 20+ (if running frontend from IDE)
