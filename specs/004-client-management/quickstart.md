# Quickstart: Client Management Development

**Feature**: 004-client-management
**Date**: 2026-02-06

## Prerequisites

- Docker Desktop running
- .NET 9 SDK installed
- Node.js 20+ installed
- Git configured

## Initial Setup

### 1. Start Infrastructure

```bash
cd transcript-project/docker
./scripts/up.sh
```

This starts PostgreSQL, Redis, Azurite, and Seq. Wait for health checks to pass.

### 2. Verify Services

```bash
./scripts/health-check.sh
```

Expected output: All services healthy.

### 3. Apply Database Migrations

```bash
cd ../backend/src/TranscriptAnalyzer.Api
dotnet ef database update
```

### 4. Run Backend

```bash
dotnet run
```

API available at: http://localhost:5100

### 5. Run Frontend (separate terminal)

```bash
cd transcript-project/frontend
npm install
npm run dev
```

Frontend available at: http://localhost:3000

## Development Workflow

### Backend Changes

```bash
cd transcript-project/backend

# Run tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~ClientsContractTests"

# Watch mode
dotnet watch run --project src/TranscriptAnalyzer.Api
```

### Frontend Changes

```bash
cd transcript-project/frontend

# Run dev server with hot reload
npm run dev

# Run tests
npm test

# Run specific test file
npm test -- --testPathPattern="clients"
```

### Database Migrations

```bash
cd transcript-project/backend/src/TranscriptAnalyzer.Api

# Create new migration
dotnet ef migrations add AddBusinessEntityTypes \
  --project ../TranscriptAnalyzer.Infrastructure

# Apply migration
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName
```

## Testing Client Management

### Create Test User Context

The dev auth mode (`USE_DEV_AUTH=true` in `.env`) bypasses Auth0. Configure test headers:

```bash
# Admin user
curl -X GET http://localhost:5100/api/v1/clients \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: Admin"

# TaxProfessional user
curl -X GET http://localhost:5100/api/v1/clients \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000002" \
  -H "X-User-Role: TaxProfessional"

# ReadOnly user
curl -X GET http://localhost:5100/api/v1/clients \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000003" \
  -H "X-User-Role: ReadOnly"
```

### Create Individual Client

```bash
curl -X POST http://localhost:5100/api/v1/clients \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: TaxProfessional" \
  -d '{
    "clientType": "Individual",
    "firstName": "John",
    "lastName": "Doe",
    "taxIdentifier": "123-45-6789",
    "email": "john.doe@example.com",
    "phone": "555-123-4567",
    "address": {
      "street1": "123 Main St",
      "city": "Anytown",
      "state": "CA",
      "postalCode": "90210",
      "country": "US"
    }
  }'
```

### Create Business Client

```bash
curl -X POST http://localhost:5100/api/v1/clients \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -H "X-User-Role: TaxProfessional" \
  -d '{
    "clientType": "Business",
    "businessName": "Acme Corporation",
    "entityType": "CCorp",
    "responsibleParty": "Jane Smith",
    "taxIdentifier": "12-3456789",
    "email": "contact@acme.com",
    "phone": "555-987-6543",
    "address": {
      "street1": "456 Corporate Blvd",
      "suite": "Suite 100",
      "city": "Enterprise",
      "state": "NY",
      "postalCode": "10001",
      "country": "US"
    }
  }'
```

### Search Clients

```bash
# Search by name
curl "http://localhost:5100/api/v1/clients?search=John" \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001"

# Search by last 4 of SSN
curl "http://localhost:5100/api/v1/clients?search=6789" \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001"

# Filter by type and sort
curl "http://localhost:5100/api/v1/clients?clientType=Business&sortBy=name&sortOrder=desc" \
  -H "Authorization: Bearer dev-token" \
  -H "X-Organization-Id: 00000000-0000-0000-0000-000000000001"
```

## Troubleshooting

### Database Connection Issues

```bash
# Check PostgreSQL is running
docker ps | grep postgres

# View PostgreSQL logs
docker logs transcript-postgres

# Connect directly to database
docker exec -it transcript-postgres psql -U postgres -d transcript_analyzer
```

### RLS Issues

```bash
# Verify tenant context is set
docker exec -it transcript-postgres psql -U postgres -d transcript_analyzer -c \
  "SELECT current_setting('app.organization_id', true);"

# Check RLS policies
docker exec -it transcript-postgres psql -U postgres -d transcript_analyzer -c \
  "SELECT * FROM pg_policies WHERE tablename = 'clients';"
```

### Encryption Key Issues

```bash
# Verify encryption key is configured
grep ENCRYPTION_KEY transcript-project/docker/.env

# Generate new key if needed
openssl rand -base64 32
```

### Frontend API Issues

```bash
# Check API is accessible
curl http://localhost:5100/health

# View API logs
cd transcript-project/backend/src/TranscriptAnalyzer.Api
dotnet run --verbosity detailed
```

## Service URLs

| Service | URL | Purpose |
|---------|-----|---------|
| API | http://localhost:5100 | Backend REST API |
| API Swagger | http://localhost:5100/swagger | API documentation |
| API Health | http://localhost:5100/health | Health check |
| Frontend | http://localhost:3000 | Next.js application |
| Seq Logs | http://localhost:8081 | Centralized logging |
| pgAdmin | http://localhost:5050 | Database admin (tools profile) |

## Key Files

### Backend

| Path | Purpose |
|------|---------|
| `backend/src/TranscriptAnalyzer.Domain/Entities/Client.cs` | Client entity |
| `backend/src/TranscriptAnalyzer.Application/Clients/` | CQRS handlers |
| `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs` | API endpoints |
| `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/` | Contract tests |

### Frontend

| Path | Purpose |
|------|---------|
| `frontend/src/app/(dashboard)/clients/` | Client pages |
| `frontend/src/components/clients/` | Client components |
| `frontend/src/lib/api/clientsApi.ts` | RTK Query API slice |
| `frontend/tests/clients/` | Component tests |

### Configuration

| Path | Purpose |
|------|---------|
| `docker/.env` | Local environment config |
| `backend/src/TranscriptAnalyzer.Api/appsettings.Development.json` | Dev settings |
| `specs/004-client-management/contracts/clients-api.yaml` | API contract |
