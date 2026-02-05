# Quickstart Guide: IRS Transcript Analysis System - MVP Foundation

**Feature**: 001-mvp-foundation
**Date**: 2026-02-05

## Prerequisites

### Required Software

- **Node.js** 20.x LTS
- **.NET SDK** 10.0
- **Docker Desktop** (for local development)
- **Azure CLI** (for deployment)
- **Git**

### Required Accounts/Services

- **Auth0** account with tenant configured
- **Azure** subscription (for deployment)
- **SendGrid** account (for email notifications)
- **DocuSign Developer** account (optional, for third-party e-signature)

## Local Development Setup

### 1. Clone and Configure

```bash
# Clone the repository
git clone https://github.com/your-org/transcript-analyzer.git
cd transcript-analyzer

# Switch to feature branch
git checkout 001-mvp-foundation

# Copy environment templates
cp backend/.env.example backend/.env
cp frontend/.env.local.example frontend/.env.local
```

### 2. Configure Environment Variables

**Backend (`backend/.env`)**:
```env
# Database (Docker Compose provides this)
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=TranscriptAnalyzer;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true

# Auth0
Auth0__Domain=your-tenant.auth0.com
Auth0__Audience=https://api.transcript-analyzer.com

# Azure Storage (Azurite for local dev)
AzureStorage__ConnectionString=UseDevelopmentStorage=true

# Encryption (generate with: openssl rand -base64 32)
Encryption__Key=your-base64-encoded-key

# SendGrid
SendGrid__ApiKey=your-sendgrid-api-key
SendGrid__FromEmail=noreply@your-domain.com

# DocuSign (optional)
DocuSign__IntegrationKey=
DocuSign__UserId=
DocuSign__AccountId=
DocuSign__AuthServer=account-d.docusign.com
```

**Frontend (`frontend/.env.local`)**:
```env
# Auth0
AUTH0_SECRET=your-long-random-secret
AUTH0_BASE_URL=http://localhost:3000
AUTH0_ISSUER_BASE_URL=https://your-tenant.auth0.com
AUTH0_CLIENT_ID=your-client-id
AUTH0_CLIENT_SECRET=your-client-secret

# API
NEXT_PUBLIC_API_URL=http://localhost:5000/api/v1
```

### 3. Start Infrastructure (Docker Compose)

```bash
# Start SQL Server, Redis, and Azurite (blob storage emulator)
docker-compose up -d

# Verify containers are running
docker-compose ps
```

**docker-compose.yml** (create if not exists):
```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong@Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    ports:
      - "10000:10000"  # Blob
      - "10001:10001"  # Queue
      - "10002:10002"  # Table
    volumes:
      - azurite-data:/data

volumes:
  sqlserver-data:
  azurite-data:
```

### 4. Setup Backend

```bash
cd backend

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/TranscriptAnalyzer.Infrastructure --startup-project src/TranscriptAnalyzer.Api

# Run backend (watches for changes)
dotnet watch run --project src/TranscriptAnalyzer.Api
```

Backend will be available at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### 5. Setup Frontend

```bash
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev
```

Frontend will be available at: `http://localhost:3000`

### 6. Configure Auth0

1. Create a new **Regular Web Application** in Auth0
2. Set allowed callback URLs: `http://localhost:3000/api/auth/callback`
3. Set allowed logout URLs: `http://localhost:3000`
4. Create an **API** with identifier: `https://api.transcript-analyzer.com`
5. Enable **RBAC** and add permissions:
   - `read:clients`
   - `write:clients`
   - `read:authorizations`
   - `write:authorizations`
   - `read:transcripts`
   - `write:transcripts`
   - `manage:organization`
6. Create roles: `Admin`, `TaxProfessional`, `ReadOnly`
7. Assign permissions to roles

## Running Tests

### Backend Tests

```bash
cd backend

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/TranscriptAnalyzer.Domain.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Tests

```bash
cd frontend

# Run unit tests
npm test

# Run with coverage
npm test -- --coverage

# Run E2E tests (requires running app)
npm run test:e2e
```

### Contract Tests

```bash
cd backend

# Run contract tests against OpenAPI specs
dotnet test tests/TranscriptAnalyzer.Contract.Tests
```

## Project Structure

```
transcript-analyzer/
├── backend/
│   ├── src/
│   │   ├── TranscriptAnalyzer.Domain/       # Entities, interfaces
│   │   ├── TranscriptAnalyzer.Application/  # Use cases, DTOs
│   │   ├── TranscriptAnalyzer.Infrastructure/ # DB, storage, services
│   │   └── TranscriptAnalyzer.Api/          # Endpoints, middleware
│   └── tests/
│       ├── TranscriptAnalyzer.Domain.Tests/
│       ├── TranscriptAnalyzer.Application.Tests/
│       ├── TranscriptAnalyzer.Infrastructure.Tests/
│       ├── TranscriptAnalyzer.Api.Tests/
│       └── TranscriptAnalyzer.Contract.Tests/
├── frontend/
│   ├── src/
│   │   ├── app/                             # Next.js pages
│   │   ├── components/                      # React components
│   │   ├── lib/                            # Utilities, API
│   │   └── store/                          # Redux store
│   └── tests/
├── specs/                                   # Feature specifications
│   └── 001-mvp-foundation/
├── docker-compose.yml
└── README.md
```

## Common Tasks

### Create a New Migration

```bash
cd backend
dotnet ef migrations add MigrationName \
  --project src/TranscriptAnalyzer.Infrastructure \
  --startup-project src/TranscriptAnalyzer.Api \
  --output-dir Persistence/Migrations
```

### Reset Database

```bash
# Drop and recreate
dotnet ef database drop --force --project src/TranscriptAnalyzer.Infrastructure --startup-project src/TranscriptAnalyzer.Api
dotnet ef database update --project src/TranscriptAnalyzer.Infrastructure --startup-project src/TranscriptAnalyzer.Api
```

### Generate API Client (Frontend)

```bash
cd frontend
npm run generate:api
```

This uses the OpenAPI specs in `specs/001-mvp-foundation/contracts/` to generate TypeScript types and RTK Query hooks.

### Check Code Quality

```bash
# Backend
cd backend
dotnet format --verify-no-changes

# Frontend
cd frontend
npm run lint
npm run type-check
```

## Deployment (Azure)

### Prerequisites

1. Azure subscription with required resources:
   - App Service Plan
   - App Service (backend)
   - Static Web App (frontend)
   - Azure SQL Database
   - Azure Blob Storage
   - Azure Key Vault
   - Redis Cache

2. GitHub Actions configured with Azure credentials

### Deploy

```bash
# Deploy to staging
git push origin 001-mvp-foundation

# GitHub Actions will:
# 1. Run tests
# 2. Build containers
# 3. Deploy to Azure staging

# After approval, promote to production
# (via GitHub environment protection rules)
```

## Troubleshooting

### Database Connection Issues

```bash
# Check SQL Server is running
docker-compose logs sqlserver

# Test connection
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -Q "SELECT 1"
```

### Auth0 Login Errors

1. Verify callback URLs match exactly
2. Check Auth0 logs in dashboard
3. Ensure `AUTH0_SECRET` is set (min 32 characters)

### Blob Storage Errors

```bash
# Check Azurite is running
docker-compose logs azurite

# List containers
az storage container list --connection-string "UseDevelopmentStorage=true"
```

## Next Steps

After completing setup:

1. Run `/speckit.tasks` to generate implementation tasks
2. Begin with Phase 1 (Setup) tasks
3. Follow TDD: write tests first, then implement
4. Commit after each completed task
