# Quickstart: Local Build Support

**Feature**: 002-local-build-support
**Date**: 2026-02-05

## Prerequisites

Before starting, ensure you have:

- [ ] Docker Desktop 4.x installed and running (or Docker Engine + Compose V2 on Linux)
- [ ] At least 8GB RAM available for Docker
- [ ] ~5GB free disk space for Docker images
- [ ] Git installed (for running shell scripts)

Optional for IDE development:
- [ ] .NET 9 SDK (if running API from IDE)
- [ ] Node.js 20+ and npm (if running frontend from IDE)
- [ ] Tilt 0.33+ (for enhanced hot-reload experience)

## Quick Start

### 1. Setup Environment

```bash
# Navigate to docker directory
cd transcript-project/docker

# Copy environment template
cp .env.template .env

# (Optional) Edit .env to customize ports or settings
```

### 2. Start Services

**Option A: Infrastructure Only (Recommended for IDE development)**
```bash
./scripts/up.sh
```
Then run API and frontend from your IDE with debugging.

**Option B: API + Infrastructure**
```bash
./scripts/up.sh api
```
Then run frontend from your IDE.

**Option C: Full Stack**
```bash
./scripts/up.sh full
```

**Option D: Full Stack + Dev Tools**
```bash
./scripts/up.sh tools
```

### 3. Verify Services

```bash
./scripts/health-check.sh
```

### 4. Access Services

| Service | URL | Notes |
|---------|-----|-------|
| API | http://localhost:5000 | When running in container |
| API Health | http://localhost:5000/health | Health check endpoint |
| Frontend | http://localhost:3000 | When running in container |
| Seq (Logs) | http://localhost:8081 | Username: admin |
| Azure Data Studio | http://localhost:5050 | Tools profile only |
| Redis Commander | http://localhost:8082 | Tools profile only |

### 5. Stop Services

```bash
# Stop services (keep data)
./scripts/down.sh

# Stop and remove all data
./scripts/down.sh -v
```

## Common Workflows

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

# In another terminal, start frontend from IDE
cd ../frontend
npm run dev
```

### Full Stack Testing

```bash
# Start everything
./scripts/up.sh full

# Make changes - hot reload is automatic
# View logs
./scripts/logs.sh api
./scripts/logs.sh web
```

### Using Tilt (Enhanced Hot Reload)

```bash
# Requires Tilt installed: https://docs.tilt.dev/install.html
./scripts/tilt-up.sh

# Open Tilt UI at http://localhost:10350
```

## Troubleshooting

### Port Already in Use

Edit `.env` to change the conflicting port:
```bash
API_PORT=5001    # Change from default 5000
WEB_PORT=3001    # Change from default 3000
```

### SQL Server Won't Start

Check Docker has enough memory:
- Docker Desktop → Settings → Resources → Memory: 8GB minimum

### Hot Reload Not Working

Ensure polling is enabled (should be automatic):
- API: `DOTNET_USE_POLLING_FILE_WATCHER=1`
- Frontend: `WATCHPACK_POLLING=true`

### View Container Logs

```bash
# All services
./scripts/logs.sh

# Specific service
./scripts/logs.sh api
./scripts/logs.sh sqlserver

# Follow logs
./scripts/logs.sh api --follow
```

### Reset Everything

```bash
# Nuclear option - removes all data
./scripts/down.sh -v
./scripts/up.sh
```

## Environment Variables Reference

| Variable | Default | Description |
|----------|---------|-------------|
| `API_PORT` | 5000 | .NET API external port |
| `WEB_PORT` | 3000 | Next.js frontend port |
| `SQL_PORT` | 1433 | SQL Server port |
| `REDIS_PORT` | 6379 | Redis port |
| `SEQ_UI_PORT` | 8081 | Seq web UI port |
| `SEQ_INGESTION_PORT` | 5341 | Seq log ingestion port |
| `AZURITE_BLOB_PORT` | 10000 | Azure Blob emulator |
| `AZURITE_QUEUE_PORT` | 10001 | Azure Queue emulator |
| `AZURITE_TABLE_PORT` | 10002 | Azure Table emulator |
| `USE_DEV_AUTH` | true | Bypass Auth0 for local dev |
| `SA_PASSWORD` | YourStrong@Passw0rd | SQL Server SA password |

## Next Steps

1. Review the full documentation in `docker/README.md`
2. Configure Auth0 settings if testing authentication
3. Set up your IDE for debugging
