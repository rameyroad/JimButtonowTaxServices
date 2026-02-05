<!--
SYNC IMPACT REPORT
==================
Version change: 0.0.0 → 1.0.0 (MAJOR - initial constitution establishment)

Modified principles: N/A (initial version)

Added sections:
- Core Principles (5): Security-First, Test-Driven Development, Clean Architecture,
  API-First Design, Scalability
- Compliance Requirements
- Development Workflow
- Governance

Removed sections: N/A (initial version)

Templates requiring updates:
- .specify/templates/plan-template.md ✅ (Constitution Check section compatible)
- .specify/templates/spec-template.md ✅ (requirements structure compatible)
- .specify/templates/tasks-template.md ✅ (phase structure compatible)
- .specify/templates/checklist-template.md ✅ (no constitution references)
- .specify/templates/agent-file-template.md ✅ (no constitution references)

Follow-up TODOs: None
-->

# Transcript Analyzer Constitution

## Core Principles

### I. Security-First

All development decisions MUST prioritize the security of taxpayer data and IRS
information. This principle is non-negotiable given the sensitive nature of the
data handled.

**Requirements:**
- All PII and tax data MUST be encrypted at rest (AES-256) and in transit (TLS 1.3+)
- Authentication MUST use Auth0/OCTA with JWT tokens; no custom auth implementations
- Authorization MUST implement role-based access control (RBAC) for all endpoints
- All data access MUST be logged with immutable audit trails
- Credentials, API keys, and secrets MUST NEVER be committed to source control
- Input validation MUST occur at all system boundaries (API endpoints, form submissions)
- SQL queries MUST use parameterized queries or Entity Framework—no string concatenation

**Rationale:** IRS transcript data contains highly sensitive PII. Breaches carry
severe legal, financial, and reputational consequences. Security cannot be an
afterthought.

### II. Test-Driven Development

Tests MUST be written before implementation code. This ensures requirements are
understood, edge cases are considered, and regressions are caught early.

**Requirements:**
- Contract tests MUST exist for all API endpoints before implementation
- Integration tests MUST cover all IRS API interactions (TDS, CAF, e-Services)
- Unit tests MUST cover business logic in services and domain models
- Test coverage MUST meet or exceed 80% for backend services
- Tests MUST fail first (Red), then pass (Green), then optimize (Refactor)
- No PR may be merged with failing tests
- Mock external services (IRS APIs) in tests; use recorded responses

**Rationale:** The IRS integration complexity and regulatory requirements demand
high confidence in correctness. TDD enforces this discipline systematically.

### III. Clean Architecture

The codebase MUST follow Clean Architecture principles to maintain separation of
concerns, testability, and long-term maintainability.

**Requirements:**
- Domain layer MUST have zero dependencies on infrastructure or frameworks
- Application layer MUST orchestrate use cases without knowledge of delivery mechanism
- Infrastructure layer MUST implement interfaces defined by inner layers
- Dependency flow MUST always point inward (UI → Application → Domain)
- Each bounded context (Authorization, Transcripts, Analysis) MUST be clearly separated
- Cross-cutting concerns (logging, caching) MUST use dependency injection
- No business logic in controllers/endpoints—delegate to application services

**Rationale:** The project spans multiple bounded contexts (8821 authorization,
transcript retrieval, multiple analysis types). Clean Architecture prevents
coupling and enables independent evolution.

### IV. API-First Design

All backend capabilities MUST be exposed through well-documented, versioned APIs
before any UI implementation begins.

**Requirements:**
- OpenAPI 3.0+ specifications MUST be written before endpoint implementation
- API versioning MUST use URL path versioning (e.g., `/api/v1/transcripts`)
- Breaking changes MUST increment the major version and maintain previous version
- Error responses MUST follow RFC 7807 Problem Details format
- All endpoints MUST be documented with request/response examples
- Rate limiting MUST be implemented for all public endpoints
- CORS policies MUST be explicitly configured per environment

**Rationale:** The system serves multiple frontends (admin, client portals) and
potentially third-party integrations. API contracts enable parallel development
and clear integration boundaries.

### V. Scalability

The system MUST be designed to handle growth in users, transcripts, and analysis
workloads without architectural changes.

**Requirements:**
- Transcript processing MUST support background/async execution for batch operations
- Database queries MUST use pagination for all list endpoints (max 100 items)
- Heavy analysis jobs MUST be offloaded to Azure Functions or background workers
- Caching MUST be implemented for frequently accessed, slowly changing data
- File storage MUST use Azure Blob Storage—no local filesystem dependencies
- Connection pooling MUST be configured for all database connections
- Horizontal scaling MUST be possible without code changes (stateless services)

**Rationale:** Tax seasons create predictable load spikes. The analysis engine
must process thousands of transcripts. Designing for scale from the start avoids
costly rewrites.

## Compliance Requirements

The Transcript Analyzer handles IRS taxpayer data and MUST comply with applicable
regulations and security standards.

**Mandatory Compliance:**
- IRS Publication 1075 requirements for safeguarding Federal Tax Information (FTI)
- SOC 2 Type II controls for service organizations
- State-specific DOR data handling requirements where applicable
- Data retention policies: minimum 7 years for tax-related records
- Right to deletion: taxpayer data removal within 30 days upon valid request
- Breach notification: within 72 hours per applicable regulations

**Audit Requirements:**
- All authentication events MUST be logged
- All data access events MUST include user ID, timestamp, and resource accessed
- Logs MUST be retained for minimum 1 year
- Quarterly access reviews MUST be conducted

## Development Workflow

All contributions MUST follow this workflow to ensure quality and traceability.

**Branch Strategy:**
- `main` branch is protected; direct commits prohibited
- Feature branches MUST use format: `###-feature-name` (e.g., `042-transcript-parser`)
- All changes require PR with at least one approval
- PRs MUST pass all CI checks before merge

**Code Review Requirements:**
- Security-sensitive changes require security-focused reviewer
- Database migrations require DBA review
- API contract changes require documentation update verification
- All feedback MUST be addressed before merge

**Quality Gates:**
- Linting (ESLint for frontend, dotnet format for backend)
- Type checking (TypeScript strict mode, C# nullable reference types)
- Test suite passes (unit, integration, contract)
- No decrease in test coverage
- Security scan (dependency vulnerabilities)

## Governance

This constitution supersedes all other development practices and guidelines for
the Transcript Analyzer project. All team members MUST comply.

**Amendment Process:**
1. Propose amendment via PR to this file with rationale
2. Obtain approval from project lead and at least one domain expert
3. Update version number per semantic versioning rules
4. Update dependent templates if affected
5. Communicate changes to all team members

**Version Policy:**
- MAJOR: Backward-incompatible changes (principle removal, redefinition)
- MINOR: New principles, sections, or material expansions
- PATCH: Clarifications, typo fixes, non-semantic refinements

**Compliance Review:**
- All PRs MUST verify alignment with constitution principles
- Quarterly constitution review to assess relevance and gaps
- Violations MUST be documented and remediated before merge
- See `README.md` for project-specific development guidance

**Version**: 1.0.0 | **Ratified**: 2026-02-05 | **Last Amended**: 2026-02-05
