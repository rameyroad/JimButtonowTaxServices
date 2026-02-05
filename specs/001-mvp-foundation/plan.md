# Implementation Plan: IRS Transcript Analysis System - MVP Foundation

**Branch**: `001-mvp-foundation` | **Date**: 2026-02-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-mvp-foundation/spec.md`

## Summary

Build the foundational scaffolding for a multi-tenant IRS transcript analysis system focused on secure client data management, 8821 authorization workflow, and transcript storage. The MVP enables tax professionals to onboard clients, collect e-signatures on Form 8821, and store transcripts securely. Analysis modules will be added incrementally in future phases. UI based on CargoMax dashboard theme.

## Technical Context

**Language/Version**:
- Frontend: TypeScript 5.x, Next.js 14, React 18
- Backend: C# 12, .NET 10.0

**Primary Dependencies**:
- Frontend: Next.js 14, Material-UI (MUI), Redux Toolkit, RTK Query, Auth0 SDK
- Backend: ASP.NET Core 10.0 (Minimal APIs), Entity Framework Core 10.0, Dapper, FluentValidation, MediatR

**Storage**:
- Azure SQL Database (relational data, multi-tenant with organization_id)
- Azure Blob Storage (encrypted transcripts and signed 8821 forms)
- Redis Cache (session caching, notification state)

**Testing**:
- Frontend: Jest, React Testing Library, Playwright (E2E)
- Backend: xUnit, NSubstitute, FluentAssertions, TestContainers

**Target Platform**: Azure Cloud (App Services, Azure Functions, Azure SQL, Blob Storage)

**Project Type**: Web application (frontend + backend)

**Performance Goals**:
- API response time < 2 seconds
- Dashboard load < 2 seconds for 1,000 clients
- Search results < 3 seconds
- 100 concurrent users

**Constraints**:
- IRS Publication 1075 compliance
- SOC 2 Type II readiness
- 7-year data retention
- AES-256 encryption at rest
- TLS 1.3+ in transit

**Scale/Scope**:
- 100+ organizations (multi-tenant)
- 1,000 clients per organization
- 10,000 transcripts/day capacity

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Security-First ✅

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| PII encrypted at rest (AES-256) | ✅ Pass | Azure SQL TDE + Blob Storage encryption |
| Encrypted in transit (TLS 1.3+) | ✅ Pass | Azure App Service enforced HTTPS |
| Auth0/OCTA with JWT | ✅ Pass | Auth0 integration per spec FR-023 |
| RBAC for all endpoints | ✅ Pass | Admin/TaxProfessional/ReadOnly roles (FR-021) |
| Immutable audit trails | ✅ Pass | AuditLog entity with before/after values (FR-006, FR-024) |
| No secrets in source control | ✅ Pass | Azure Key Vault + environment variables |
| Input validation at boundaries | ✅ Pass | FluentValidation on all API inputs |
| Parameterized queries | ✅ Pass | Entity Framework Core + Dapper parameterized |

### II. Test-Driven Development ✅

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| Contract tests before implementation | ✅ Pass | OpenAPI spec + contract tests in /tests/contract |
| Integration tests for external APIs | ✅ Pass | E-signature provider mocked, notification services |
| Unit tests for business logic | ✅ Pass | Domain services covered |
| 80% coverage for backend | ✅ Target | CI gate enforced |
| Red-Green-Refactor | ✅ Process | Task workflow enforces test-first |
| No merge with failing tests | ✅ Pass | GitHub Actions CI gate |
| Mock external services | ✅ Pass | E-signature, email services mocked |

### III. Clean Architecture ✅

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| Domain layer zero dependencies | ✅ Pass | Domain project has no external refs |
| Application orchestrates use cases | ✅ Pass | MediatR handlers in Application layer |
| Infrastructure implements interfaces | ✅ Pass | Repositories, services implement domain interfaces |
| Dependency flow inward | ✅ Pass | Project references enforce direction |
| Bounded contexts separated | ✅ Pass | Authorization, Clients, Transcripts, Organizations |
| DI for cross-cutting concerns | ✅ Pass | Logging, caching via DI |
| No business logic in controllers | ✅ Pass | Minimal API endpoints delegate to handlers |

### IV. API-First Design ✅

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| OpenAPI 3.0+ specs first | ✅ Pass | /contracts/ contains OpenAPI specs |
| URL path versioning | ✅ Pass | /api/v1/* pattern |
| Breaking changes = major version | ✅ Pass | Versioning policy documented |
| RFC 7807 error responses | ✅ Pass | ProblemDetails middleware |
| Documented with examples | ✅ Pass | OpenAPI includes examples |
| Rate limiting | ✅ Pass | Azure API Management |
| CORS configured per environment | ✅ Pass | Environment-specific CORS policies |

### V. Scalability ✅

| Requirement | Status | Implementation |
|-------------|--------|----------------|
| Background/async for batch ops | ✅ Pass | Azure Functions for notifications |
| Pagination (max 100) | ✅ Pass | All list endpoints paginated |
| Heavy jobs to background workers | ✅ Pass | Azure Functions (future analysis) |
| Caching for slow-changing data | ✅ Pass | Redis for organization settings, lookups |
| Azure Blob Storage (no local FS) | ✅ Pass | All files in Blob Storage |
| Connection pooling | ✅ Pass | EF Core connection pooling |
| Stateless services | ✅ Pass | No server-side session state |

## Project Structure

### Documentation (this feature)

```text
specs/001-mvp-foundation/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (OpenAPI specs)
│   ├── organizations-api.yaml
│   ├── clients-api.yaml
│   ├── authorizations-api.yaml
│   ├── transcripts-api.yaml
│   └── notifications-api.yaml
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── TranscriptAnalyzer.Domain/           # Entities, value objects, domain events
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   └── Interfaces/
│   ├── TranscriptAnalyzer.Application/      # Use cases, DTOs, validators
│   │   ├── Common/
│   │   ├── Organizations/
│   │   ├── Clients/
│   │   ├── Authorizations/
│   │   ├── Transcripts/
│   │   └── Notifications/
│   ├── TranscriptAnalyzer.Infrastructure/   # EF Core, Blob Storage, external services
│   │   ├── Persistence/
│   │   ├── Storage/
│   │   ├── Identity/
│   │   ├── Notifications/
│   │   └── ESignature/
│   └── TranscriptAnalyzer.Api/              # Minimal API endpoints, middleware
│       ├── Endpoints/
│       ├── Middleware/
│       └── Configuration/
└── tests/
    ├── TranscriptAnalyzer.Domain.Tests/
    ├── TranscriptAnalyzer.Application.Tests/
    ├── TranscriptAnalyzer.Infrastructure.Tests/
    ├── TranscriptAnalyzer.Api.Tests/
    └── TranscriptAnalyzer.Contract.Tests/

frontend/
├── src/
│   ├── app/                    # Next.js App Router
│   │   ├── (auth)/            # Auth routes (login, callback)
│   │   ├── (dashboard)/       # Protected dashboard routes
│   │   │   ├── clients/
│   │   │   ├── authorizations/
│   │   │   ├── transcripts/
│   │   │   ├── settings/
│   │   │   └── team/
│   │   └── (public)/          # Public routes (signature page)
│   ├── components/
│   │   ├── ui/                # Base UI components (from CargoMax)
│   │   ├── clients/
│   │   ├── authorizations/
│   │   ├── transcripts/
│   │   └── shared/
│   ├── lib/
│   │   ├── api/               # RTK Query API slices
│   │   ├── auth/              # Auth0 utilities
│   │   └── utils/
│   ├── store/                 # Redux store configuration
│   └── types/                 # TypeScript type definitions
└── tests/
    ├── unit/
    ├── integration/
    └── e2e/

shared/
└── contracts/                 # Shared OpenAPI specs (copied from specs/)
```

**Structure Decision**: Web application structure selected based on Next.js frontend + .NET backend architecture specified in README.md. Clean Architecture pattern for backend with four layers (Domain, Application, Infrastructure, Api). Frontend follows Next.js 14 App Router conventions with feature-based organization matching the CargoMax dashboard theme structure.

## Complexity Tracking

> No constitution violations requiring justification. All principles satisfied by design.

| Decision | Rationale | Alternative Considered |
|----------|-----------|------------------------|
| Four backend projects | Clean Architecture separation | Monolith rejected for testability |
| Redis cache | Session + notification state | In-memory rejected for horizontal scaling |
| Azure Functions (future) | Analysis job scalability | Background threads rejected for reliability |
