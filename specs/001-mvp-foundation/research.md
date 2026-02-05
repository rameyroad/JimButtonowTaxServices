# Research: IRS Transcript Analysis System - MVP Foundation

**Feature**: 001-mvp-foundation
**Date**: 2026-02-05

## Technology Decisions

### 1. E-Signature Provider Integration

**Decision**: Abstract e-signature behind `IESignatureService` interface with two implementations:
1. Built-in simple e-signature (typed name + checkbox + timestamp/IP)
2. DocuSign/Adobe Sign integration (configurable per organization)

**Rationale**:
- Organizations can start with built-in (lower cost, faster setup)
- Upgrade to third-party for enhanced compliance when needed
- IRS accepts electronic signatures per IRS Pub 4557 when proper consent is captured
- Interface abstraction allows easy addition of new providers

**Alternatives Considered**:
- DocuSign-only: Rejected due to cost for small firms
- HelloSign: Lower market share, fewer integrations
- PandaDoc: Less IRS-specific compliance documentation

**Implementation Notes**:
- Built-in: Capture typed name, checkbox consent, timestamp (UTC), IP address, user agent
- Store signature metadata as JSON blob in Authorization record
- DocuSign: Use eSignature REST API with embedded signing
- Configuration stored at Organization level (`preferred_esignature_provider`)

### 2. Multi-Tenant Data Isolation Strategy

**Decision**: Logical multi-tenancy with `OrganizationId` column on all tenant-scoped tables + row-level security (RLS) in Azure SQL.

**Rationale**:
- Single database reduces operational complexity
- RLS enforces isolation at database level (defense in depth)
- EF Core global query filters ensure application-level isolation
- Cost-effective for projected 100+ organizations

**Alternatives Considered**:
- Database-per-tenant: Higher cost, operational overhead
- Schema-per-tenant: Complex migrations
- Shared tables without RLS: Security risk

**Implementation Notes**:
- `OrganizationId` column on: Clients, Users, Authorizations, Transcripts, AuditLogs
- Global EF Core query filter: `.HasQueryFilter(e => e.OrganizationId == _tenantId)`
- Azure SQL RLS policies for direct query protection
- Tenant context extracted from JWT claims in middleware

### 3. PII Encryption Strategy

**Decision**: Two-tier encryption approach:
1. Azure SQL Transparent Data Encryption (TDE) for at-rest encryption
2. Application-level field encryption for SSN/EIN using Azure Key Vault managed keys

**Rationale**:
- TDE provides baseline encryption without code changes
- Field-level encryption ensures PII is encrypted even in backups/exports
- Key Vault provides centralized key management with audit trail
- Meets IRS Pub 1075 requirements for FTI protection

**Alternatives Considered**:
- Always Encrypted: More complex, driver compatibility issues
- Custom encryption: Reinventing the wheel, potential vulnerabilities
- TDE-only: Insufficient for highly sensitive fields

**Implementation Notes**:
- SSN/EIN stored as encrypted bytes, decrypted only in application
- `EncryptedString` value object wraps encryption/decryption logic
- Separate keys for SSN vs other PII (key rotation flexibility)
- Audit log captures decryption events (who viewed SSN)

### 4. Notification Architecture

**Decision**: Event-driven notification system with:
1. In-app notifications via SignalR (real-time) + database (persistence)
2. Email notifications via transactional email service (SendGrid/Azure Communication Services)
3. SMS placeholder architecture for future implementation

**Rationale**:
- SignalR provides real-time updates when user is online
- Database storage ensures notifications aren't lost if offline
- Email reaches users regardless of app state
- SMS architecture ready but not implemented (cost, complexity)

**Alternatives Considered**:
- Polling: Inefficient, delayed updates
- WebSockets (raw): More complex than SignalR abstraction
- Push notifications: Requires mobile app (future)

**Implementation Notes**:
- `NotificationService` dispatches to multiple channels
- `INotificationChannel` interface: `InAppChannel`, `EmailChannel`, `SmsChannel` (stub)
- Organization settings control which channels are enabled
- Notification templates stored in database for customization

### 5. Authorization Link Security

**Decision**: Secure, time-limited links for 8821 signature using:
- Cryptographically random token (32 bytes, URL-safe base64)
- Configurable expiration (default 7 days, org-configurable)
- Single-use invalidation after signature
- Rate limiting on link validation endpoint

**Rationale**:
- Random tokens prevent enumeration attacks
- Expiration limits exposure window
- Single-use prevents replay attacks
- Rate limiting prevents brute force attempts

**Alternatives Considered**:
- JWT-based links: Larger URLs, harder to revoke
- Sequential IDs: Enumeration vulnerability
- No expiration: Security risk

**Implementation Notes**:
- Token stored hashed in database (bcrypt)
- Link format: `https://{domain}/sign/{token}`
- Expiration check in middleware before rendering form
- IP address logged on access for audit trail

### 6. File Storage Strategy

**Decision**: Azure Blob Storage with:
- Separate containers per content type (transcripts, signed-forms)
- Blob-level encryption with customer-managed keys
- SAS tokens for time-limited download access
- Logical path: `{org_id}/{client_id}/{year}/{filename}`

**Rationale**:
- Blob Storage is cost-effective for large files
- Container separation enables different access policies
- SAS tokens provide secure, auditable downloads
- Logical paths enable efficient querying without database lookup

**Alternatives Considered**:
- Azure Files: Higher cost, unnecessary features
- Database BLOBs: Performance issues, backup complexity
- S3: Not Azure-native, additional complexity

**Implementation Notes**:
- Transcripts container: Hot tier (frequent access during analysis)
- Signed forms container: Cool tier (infrequent access after signing)
- 7-year retention policy enforced at container level
- Soft delete enabled for compliance (accidental deletion recovery)

### 7. Audit Logging Strategy

**Decision**: Comprehensive audit logging with:
- Database table for structured audit records
- Captures: user, action, timestamp, entity type, entity ID, before/after JSON
- Immutable (soft-delete only, no updates)
- Azure Monitor integration for security alerting

**Rationale**:
- IRS Pub 1075 requires audit trail for FTI access
- SOC 2 requires change tracking
- Before/after enables rollback analysis
- Azure Monitor enables real-time alerting

**Alternatives Considered**:
- Event sourcing: Overkill for audit requirements
- Log files only: Harder to query, less structured
- Third-party SIEM: Additional cost, complexity

**Implementation Notes**:
- EF Core interceptors automatically log entity changes
- Custom `AuditContext` captures user info from JWT
- Sensitive fields redacted in audit (show "SSN changed" not values)
- Retention: 7 years (same as data retention)

### 8. CargoMax Theme Adaptation

**Decision**: Fork and adapt CargoMax dashboard components:
- Copy UI components to `frontend/src/components/ui/`
- Retheme colors/branding for tax professional context
- Reuse navigation, table, card, and form patterns
- Remove logistics-specific components (maps, fleet tracking)

**Rationale**:
- CargoMax provides production-ready dashboard foundation
- Consistent design language accelerates development
- Tailwind + shadcn/ui components are highly customizable
- Saves significant frontend development time

**Alternatives Considered**:
- Build from scratch: Time-consuming, inconsistent
- Material-UI templates: Different design system
- Ant Design: Heavier bundle, less customization

**Implementation Notes**:
- Theme colors: Navy/blue (trust/professional) primary
- Sidebar navigation maps to bounded contexts
- Client list reuses CargoMax data table component
- Dashboard widgets adapt metric card patterns

## Best Practices Applied

### .NET Clean Architecture

- **Domain**: Entities with business logic, value objects (SSN, EIN), domain events
- **Application**: CQRS with MediatR (Commands/Queries), DTOs, validators
- **Infrastructure**: EF Core repos, Blob Storage, external service adapters
- **API**: Minimal APIs with endpoint classes, middleware pipeline

### Next.js App Router Patterns

- Route groups for layout organization: `(auth)`, `(dashboard)`, `(public)`
- Server Components for initial load, Client Components for interactivity
- RTK Query for data fetching with caching
- Middleware for auth protection

### Security Patterns

- Defense in depth: Auth0 + JWT validation + RBAC + RLS
- Principle of least privilege: Scoped tokens, minimal permissions
- Input validation: FluentValidation + client-side validation
- Output encoding: React's built-in XSS protection

## Dependencies

### Backend NuGet Packages

```xml
<!-- Domain (none - pure C#) -->

<!-- Application -->
<PackageReference Include="MediatR" Version="12.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="AutoMapper" Version="13.*" />

<!-- Infrastructure -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.*" />
<PackageReference Include="Dapper" Version="2.*" />
<PackageReference Include="Azure.Storage.Blobs" Version="12.*" />
<PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.*" />
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="10.*" />
<PackageReference Include="SendGrid" Version="9.*" />

<!-- API -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
```

### Frontend npm Packages

```json
{
  "dependencies": {
    "next": "^14.0.0",
    "react": "^18.2.0",
    "@reduxjs/toolkit": "^2.0.0",
    "@mui/material": "^5.15.0",
    "@auth0/nextjs-auth0": "^3.5.0",
    "tailwindcss": "^3.4.0",
    "zod": "^3.22.0"
  },
  "devDependencies": {
    "typescript": "^5.3.0",
    "jest": "^29.7.0",
    "@testing-library/react": "^14.1.0",
    "@playwright/test": "^1.40.0"
  }
}
```

## Open Questions (Resolved)

| Question | Resolution |
|----------|------------|
| E-signature provider? | Both built-in and third-party (org choice) |
| Link expiration? | Configurable per org (default 7 days) |
| Multi-tenancy approach? | Logical (shared DB, org_id column, RLS) |
| Client type model? | Unified with type flag (Individual/Business) |
| Notification channels? | Email + in-app (MVP), SMS architecture ready |

## Next Steps

1. Create data-model.md with entity definitions
2. Generate OpenAPI contracts in /contracts/
3. Create quickstart.md with setup instructions
4. Run /speckit.tasks to generate implementation tasks
