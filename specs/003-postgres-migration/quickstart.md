# Quickstart: PostgreSQL Local Development

**Date**: 2026-02-06
**Branch**: `003-postgres-migration`

## Prerequisites

- Docker Desktop installed and running
- .NET 9.0 SDK
- Git

## Quick Start (< 5 minutes)

### 1. Start PostgreSQL

```bash
cd transcript-project/docker
./scripts/up.sh
```

This starts:
- PostgreSQL 16 on port 5432
- Redis on port 6379
- Azurite (Azure Storage emulator) on ports 10000-10002
- Seq (logging) on port 8081

### 2. Verify Database

```bash
./scripts/health-check.sh
```

Expected output:
```
✓ postgres: healthy
✓ redis: healthy
✓ azurite: running
✓ seq: running
```

### 3. Run Migrations

```bash
cd ../backend
dotnet ef database update --project src/TranscriptAnalyzer.Infrastructure
```

### 4. Start the API

```bash
dotnet run --project src/TranscriptAnalyzer.Api
```

API available at: http://localhost:5100

### 5. Verify Health

```bash
curl http://localhost:5100/health
```

Expected: `{"status":"Healthy","timestamp":"..."}`

---

## Development Profiles

### Infrastructure Only (Default)

For running API/frontend from IDE:

```bash
./scripts/up.sh
# Then run API from Visual Studio/Rider/VS Code
```

### Full Stack in Docker

```bash
./scripts/up.sh full
```

Services:
- API: http://localhost:5100
- Web: http://localhost:3000

### With Dev Tools

```bash
./scripts/up.sh tools
```

Additional services:
- pgAdmin: http://localhost:5050
- Redis Commander: http://localhost:8082
- Seq Logs: http://localhost:8081

---

## Database Connection

### Default Connection String

```text
Host=localhost;Port=5432;Database=transcript_analyzer;Username=postgres;Password=YourStrong@Passw0rd
```

### Environment Variables

Create `.env` from template:

```bash
cp .env.template .env
```

Key variables:
```bash
POSTGRES_PASSWORD=YourStrong@Passw0rd
POSTGRES_DB=transcript_analyzer
POSTGRES_PORT=5432
```

---

## pgAdmin Access

When running with `tools` profile:

1. Open http://localhost:5050
2. Login: admin@local.dev / admin
3. Add server:
   - Host: postgres
   - Port: 5432
   - Username: postgres
   - Password: (from .env)

---

## Common Commands

### View Logs

```bash
./scripts/logs.sh postgres     # PostgreSQL logs
./scripts/logs.sh api          # API logs
./scripts/logs.sh --follow     # Follow all logs
```

### Stop Services

```bash
./scripts/down.sh              # Stop containers
./scripts/down.sh -v           # Stop and remove volumes
```

### Reset Database

```bash
./scripts/down.sh -v
./scripts/up.sh
cd ../backend
dotnet ef database update --project src/TranscriptAnalyzer.Infrastructure
```

### Create Migration

```bash
cd backend
dotnet ef migrations add MigrationName --project src/TranscriptAnalyzer.Infrastructure
```

---

## Tilt (Hot Reload)

For enhanced development with web UI:

```bash
./scripts/tilt-up.sh
```

Tilt UI: http://localhost:10350

Features:
- Automatic rebuild on file changes
- Service status dashboard
- Log aggregation
- Resource dependency visualization

---

## Troubleshooting

### Port Conflicts

If port 5432 is in use:

```bash
# Check what's using the port
lsof -i :5432

# Use alternate port in .env
POSTGRES_PORT=5433
```

### Connection Refused

1. Check container is running:
   ```bash
   docker ps | grep postgres
   ```

2. Check health:
   ```bash
   ./scripts/health-check.sh --wait
   ```

3. Check logs:
   ```bash
   ./scripts/logs.sh postgres
   ```

### Migration Failures

1. Ensure database exists:
   ```bash
   docker exec -it transcript-postgres psql -U postgres -c "\l"
   ```

2. Check connection string in appsettings.Development.json

3. Try fresh start:
   ```bash
   ./scripts/down.sh -v
   ./scripts/up.sh
   ```

---

## Row-Level Security (RLS)

PostgreSQL Row-Level Security provides tenant isolation at the database level.

### How It Works

1. Each request sets `app.current_tenant_id` via `TenantConnectionInterceptor`
2. RLS policies filter data based on this session variable
3. Users can only see/modify their organization's data

### Testing RLS Manually

```bash
# Connect as app_user role
docker exec -it transcript-postgres psql -U postgres -d transcript_analyzer

# Switch to app_user role
SET ROLE app_user;

# Set tenant context
SELECT set_config('app.current_tenant_id', 'your-org-id', false);

# Query - only sees data for that tenant
SELECT * FROM users;
```

### Admin Operations (Bypass RLS)

For maintenance operations that need cross-tenant access:

```bash
# Connect with app_admin role (has BYPASSRLS)
SET ROLE app_admin;

# Can see all data
SELECT * FROM users;
```

---

## Environment Comparison

| Setting | Local | Azure |
|---------|-------|-------|
| Host | localhost | *.postgres.database.azure.com |
| Port | 5432 | 5432 |
| SSL | Optional | Required |
| Username | postgres | appuser |
| Database | transcript_analyzer | transcript_analyzer |
| Connection Pooling | Default | MinPoolSize=1, MaxPoolSize=100 |

### Azure Connection String Format

```text
Host=myserver.postgres.database.azure.com;Port=5432;Database=transcript_analyzer;Username=appuser;Password=xxx;SSLMode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=1;MaxPoolSize=100
```

### Azure Tier Connection Limits

| Tier | Max Connections |
|------|-----------------|
| Burstable B1ms | 50 |
| Burstable B2s | 100 |
| General Purpose D2s_v3 | 859 |
| Memory Optimized E2s_v3 | 1719 |
