# Implementation Plan: PostgreSQL Migration

**Branch**: `003-postgres-migration` | **Date**: 2026-02-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/003-postgres-migration/spec.md`

## Summary

Migrate the Transcript Analyzer database from SQL Server to PostgreSQL with three key enhancements:
1. **PostgreSQL 16** as the database engine with Docker for local development
2. **Snake_case naming conventions** for all database objects using EF Core conventions
3. **Row-Level Security (RLS)** for database-enforced multi-tenant isolation

This is an infrastructure-layer change with no modifications to domain entities or API contracts.

## Technical Context

**Language/Version**: C# / .NET 9.0
**Primary Dependencies**:
- Npgsql.EntityFrameworkCore.PostgreSQL 9.0.x (replacing Microsoft.EntityFrameworkCore.SqlServer)
- EFCore.NamingConventions (for snake_case)
**Storage**: PostgreSQL 16 (Alpine) via Docker; Azure PostgreSQL Flexible Server for production
**Testing**: xUnit, FluentAssertions, integration tests for RLS validation
**Target Platform**: Linux containers (Docker), Azure App Service
**Project Type**: Web application (backend focus for this feature)
**Performance Goals**: Database startup < 60s, connection acquisition < 100ms
**Constraints**: Connection pool reset on each acquisition (security), SSL required for production
**Scale/Scope**: Multi-tenant SaaS, 6 tenant-scoped tables requiring RLS

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| **I. Security-First** | ✅ PASS | RLS adds defense-in-depth for tenant isolation; separate admin connection prevents bypass |
| **II. Test-Driven Development** | ✅ PASS | Integration tests will verify RLS policies before deployment |
| **III. Clean Architecture** | ✅ PASS | Changes confined to Infrastructure layer; Domain entities unchanged |
| **IV. API-First Design** | ✅ N/A | No API changes in this feature |
| **V. Scalability** | ✅ PASS | PostgreSQL supports connection pooling; configured via Npgsql |

**Compliance Notes**:
- Audit logging structure preserved (audit_logs table with RLS)
- Encryption at rest delegated to Azure PostgreSQL (transparent data encryption)
- Connection pooling with context reset prevents tenant data leakage

## Project Structure

### Documentation (this feature)

```text
specs/003-postgres-migration/
├── plan.md              # This file
├── research.md          # Phase 0: Npgsql conventions, RLS patterns
├── data-model.md        # Phase 1: Snake_case entity mappings
├── quickstart.md        # Phase 1: Local PostgreSQL setup guide
└── tasks.md             # Phase 2: Implementation tasks (created by /speckit.tasks)
```

### Source Code (repository root)

```text
transcript-project/
├── backend/
│   └── src/
│       └── TranscriptAnalyzer.Infrastructure/
│           ├── Persistence/
│           │   ├── ApplicationDbContext.cs          # Update: PostgreSQL config, naming conventions
│           │   ├── TenantContext.cs                 # Update: RLS session variable handling
│           │   ├── Configurations/                  # Update: Remove explicit table names (use conventions)
│           │   └── Migrations/                      # New: Initial PostgreSQL migration with RLS
│           └── DependencyInjection.cs               # Update: Npgsql provider, admin connection
│
├── docker/
│   ├── docker-compose.yml                           # Update: PostgreSQL service, pgAdmin
│   ├── tilt-compose.yml                             # Update: PostgreSQL service
│   ├── .env.template                                # Update: PostgreSQL connection vars
│   └── scripts/
│       └── *.sh                                     # Update: PostgreSQL references
│
└── backend/tests/
    └── TranscriptAnalyzer.Infrastructure.Tests/     # New: RLS integration tests
```

**Structure Decision**: Existing Clean Architecture structure maintained. Changes are isolated to Infrastructure layer and Docker configuration.

## Complexity Tracking

No constitution violations requiring justification. This feature simplifies the architecture by:
- Using database-native RLS instead of application-only query filters
- Using PostgreSQL conventions instead of explicit naming in each configuration

---

## Post-Design Constitution Re-Check

*Verified after Phase 1 design completion.*

| Principle | Status | Post-Design Evidence |
|-----------|--------|----------------------|
| **I. Security-First** | ✅ PASS | RLS policies defined in data-model.md; connection interceptor pattern in research.md; separate admin role documented |
| **II. Test-Driven Development** | ✅ PASS | RLS integration tests specified in project structure; test patterns researched |
| **III. Clean Architecture** | ✅ PASS | All changes in Infrastructure layer per project structure; Domain untouched |
| **IV. API-First Design** | ✅ N/A | Confirmed no API changes in contracts/README.md |
| **V. Scalability** | ✅ PASS | Connection pooling configuration documented in research.md |

**Compliance Verification**:
- ✅ Audit trail: audit_logs table with RLS (data-model.md)
- ✅ Encryption: Azure PostgreSQL TDE + application-level for TaxIdentifier
- ✅ Connection security: SSL required for production (research.md)
- ✅ Role separation: app_user (RLS) vs app_admin (bypass) documented
