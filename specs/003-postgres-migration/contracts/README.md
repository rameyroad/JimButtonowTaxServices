# API Contracts: PostgreSQL Migration

**Date**: 2026-02-06
**Branch**: `003-postgres-migration`

## No API Changes

This feature is an infrastructure-layer migration. No API contracts are modified.

### What Changes
- Database engine: SQL Server → PostgreSQL
- Naming convention: PascalCase → snake_case
- Security: Application filters → RLS + Application filters

### What Does NOT Change
- API endpoints
- Request/response schemas
- Authentication/authorization flow
- Client-facing behavior

## Connection String Configuration

The only external-facing change is the connection string format in configuration:

**Before (SQL Server):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TranscriptAnalyzer;User=sa;Password=xxx;TrustServerCertificate=true"
  }
}
```

**After (PostgreSQL):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=transcript_analyzer;Username=postgres;Password=xxx",
    "AdminConnection": "Host=localhost;Port=5432;Database=transcript_analyzer;Username=transcript_admin;Password=xxx"
  }
}
```

Note: `AdminConnection` is new and used for maintenance operations that bypass RLS.
