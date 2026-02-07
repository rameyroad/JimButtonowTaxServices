# Research: Client Management

**Feature**: 004-client-management
**Date**: 2026-02-06
**Spec**: [spec.md](./spec.md)

## Summary

Research phase for implementing client management with Individual and Business taxpayer types, PII encryption/masking, and role-based access control. The existing codebase provides a strong foundation with most domain entities and infrastructure already in place.

---

## Technical Context Analysis

### Existing Domain Model

**Location**: `backend/src/TranscriptAnalyzer.Domain/`

| Component | Status | Notes |
|-----------|--------|-------|
| `Client.cs` | ✅ Exists | Supports Individual/Business types, has factory methods |
| `Address.cs` | ✅ Exists | Value object with full address components |
| `EncryptedString.cs` | ✅ Exists | Value object for encrypted PII |
| `ClientType.cs` | ✅ Exists | Enum: Individual=0, Business=1 |
| `BusinessEntityType.cs` | ⚠️ Needs Extension | Missing: NonProfit, Trust, Estate |
| `UserRole.cs` | ✅ Exists | ReadOnly=0, TaxProfessional=1, Admin=2 |

**Gap Identified**: `BusinessEntityType` enum needs 3 additional values per FR-010:
- Current: SoleProprietor, LLC, Partnership, SCorp, CCorp (5 values)
- Required: Sole Proprietorship, Partnership, S-Corporation, C-Corporation, LLC, Non-Profit, Trust, Estate (8 values)

### Existing Infrastructure

**Encryption Service** (`Infrastructure/Security/EncryptionService.cs`):
- AES-256 encryption with random IV prepending
- SHA-256 hashing with salting
- Configured via `Encryption:Key` in appsettings

**EF Core Configuration** (`Infrastructure/Persistence/Configurations/ClientConfiguration.cs`):
- EncryptedString conversion already implemented
- TaxIdentifierLast4 indexed for search
- Address configured as owned entity

**Multi-Tenancy**:
- PostgreSQL Row-Level Security (RLS) implemented in 003-postgres-migration
- TenantMiddleware extracts `org_id` from JWT
- TenantContext uses AsyncLocal for context propagation

### Existing API Patterns

**Location**: `backend/src/TranscriptAnalyzer.Api/`

| Pattern | Implementation |
|---------|----------------|
| API Style | Minimal APIs |
| Response | Result<T> pattern |
| Pagination | PaginatedList<T> with max 100 items |
| Validation | FluentValidation + MediatR pipeline |
| Errors | RFC 7807 ProblemDetails |
| Auth | JWT via Auth0 + dev bypass mode |
| RBAC | Permission-based policies (read:clients, write:clients) |

**Existing Contract** (`specs/001-mvp-foundation/contracts/clients-api.yaml`):
- Full CRUD endpoints defined
- Pagination, search, sorting specified
- ⚠️ EntityType enum needs update to match spec

### Existing Frontend Patterns

**Location**: `frontend/src/`

| Pattern | Implementation |
|---------|----------------|
| Framework | Next.js 14 (App Router) |
| Styling | Tailwind CSS + Material-UI |
| State | Redux Toolkit + RTK Query |
| Layout | Dashboard with fixed sidebar |
| Auth | Auth0 via @auth0/nextjs-auth0 |

**CargoMax Patterns Identified**:
- Fixed 240px sidebar navigation
- Material-UI ListItemButton for nav items
- Route groups: (auth), (dashboard), (public)
- Table/list patterns from MUI DataGrid

---

## Constitution Compliance Check

### I. Security-First ✅

| Requirement | Compliance |
|-------------|------------|
| PII encrypted at rest (AES-256) | ✅ EncryptedString + EncryptionService |
| Authorization via RBAC | ✅ Permission policies exist |
| Audit trails | ✅ AuditLog entity exists |
| Input validation | ✅ FluentValidation configured |
| Parameterized queries | ✅ EF Core used |

### II. Test-Driven Development 📋

| Requirement | Plan |
|-------------|------|
| Contract tests before implementation | Create tests first per TDD |
| 80% coverage | Track during implementation |
| Integration tests | Add for client endpoints |

### III. Clean Architecture ✅

| Requirement | Compliance |
|-------------|------------|
| Domain layer independent | ✅ No infrastructure deps |
| Application orchestrates use cases | ✅ MediatR CQRS pattern |
| Infrastructure implements interfaces | ✅ IEncryptionService, ITenantContext |

### IV. API-First Design ✅

| Requirement | Compliance |
|-------------|------------|
| OpenAPI specs before implementation | ✅ clients-api.yaml exists |
| URL path versioning | ✅ /api/v1/ prefix |
| RFC 7807 errors | ✅ ProblemDetails configured |

### V. Scalability ✅

| Requirement | Compliance |
|-------------|------------|
| Pagination (max 100) | ✅ PaginatedList enforces |
| Stateless services | ✅ No session state |
| Azure Blob Storage | ✅ IBlobStorageService exists |

---

## Unknowns Resolved

### Q1: How should duplicate tax identifiers be handled?

**Decision**: Warn on creation, allow save with confirmation.

**Implementation**:
- Backend: Check for duplicate TaxIdentifierLast4 within organization
- Return 409 Conflict with details if duplicate found
- Frontend: Show warning dialog, allow user to proceed or cancel

### Q2: How should archived clients be queried?

**Decision**: Use query parameter `?includeArchived=true` (Admin only).

**Implementation**:
- Default: `WHERE deleted_at IS NULL`
- With flag: Include soft-deleted records
- UI: Toggle "Show Archived" for Admin role

### Q3: Should tax identifier updates require re-encryption?

**Decision**: Yes, full re-encryption with new IV.

**Implementation**:
- UpdateClient command accepts plain text tax identifier
- Service encrypts with new IV before saving
- Last4 updated accordingly
- Audit log captures change (without revealing old/new values)

---

## Required Changes

### Backend Domain

1. **Extend BusinessEntityType enum**:
   ```csharp
   public enum BusinessEntityType
   {
       SoleProprietor = 0,
       Partnership = 1,
       SCorp = 2,
       CCorp = 3,
       LLC = 4,
       NonProfit = 5,  // NEW
       Trust = 6,      // NEW
       Estate = 7      // NEW
   }
   ```

2. **Add soft delete to Client entity**:
   - Already inherits from SoftDeletableEntity ✅
   - Add `Archive()` and `Restore()` methods

### Backend Application

1. **Create CQRS commands/queries**:
   - `CreateClientCommand` / `CreateClientHandler`
   - `UpdateClientCommand` / `UpdateClientHandler`
   - `GetClientQuery` / `GetClientHandler`
   - `ListClientsQuery` / `ListClientsHandler`
   - `ArchiveClientCommand` / `ArchiveClientHandler`
   - `RestoreClientCommand` / `RestoreClientHandler`

2. **Create validators**:
   - `CreateClientValidator` (SSN/EIN format, required fields)
   - `UpdateClientValidator` (optional fields, format when present)

3. **Create DTOs**:
   - `ClientDto`, `ClientDetailDto`, `ClientListItemDto`
   - `CreateClientRequest`, `UpdateClientRequest`

### Backend API

1. **Implement endpoints**:
   - `GET /api/v1/clients` - List with pagination, search, sort
   - `POST /api/v1/clients` - Create
   - `GET /api/v1/clients/{id}` - Detail
   - `PATCH /api/v1/clients/{id}` - Update
   - `DELETE /api/v1/clients/{id}` - Archive (Admin only)
   - `POST /api/v1/clients/{id}/restore` - Restore (Admin only)

2. **Add authorization checks**:
   - ReadOnly: GET only
   - TaxProfessional: GET, POST, PATCH
   - Admin: All operations

### Frontend

1. **Create components**:
   - `ClientList` - Data table with pagination, search, sort
   - `ClientForm` - Create/edit form with validation
   - `ClientDetail` - View with edit button (role-dependent)
   - `TaxIdentifierInput` - Masked input with format validation
   - `ClientTypeBadge` - Individual/Business indicator

2. **Create pages**:
   - `/clients` - List page
   - `/clients/new` - Create page
   - `/clients/[id]` - Detail page
   - `/clients/[id]/edit` - Edit page

3. **Create API slice**:
   - RTK Query slice for clients CRUD
   - Cache invalidation on mutations

### Contract Updates

1. **Update EntityType enum** in `clients-api.yaml`:
   ```yaml
   EntityType:
     type: string
     enum: [SoleProprietor, Partnership, SCorp, CCorp, LLC, NonProfit, Trust, Estate]
   ```

2. **Add restore endpoint**:
   ```yaml
   /clients/{clientId}/restore:
     post:
       summary: Restore archived client
       operationId: restoreClient
   ```

3. **Add includeArchived parameter** to list endpoint

---

## Testing Strategy

### Contract Tests (Write First)

1. **List clients**: Pagination, search, sort, role restrictions
2. **Create client**: Individual, Business, validation errors, duplicate warning
3. **Get client**: Found, not found, cross-tenant (404)
4. **Update client**: Success, validation, concurrent edit
5. **Archive client**: Admin only, ReadOnly/TaxProfessional rejected
6. **Restore client**: Admin only, restores soft-deleted record

### Integration Tests

1. **Encryption round-trip**: Create → Read → Verify masked display
2. **RLS enforcement**: User A cannot access User B's clients
3. **Audit logging**: Changes recorded with user/timestamp

### Frontend Tests

1. **Form validation**: SSN/EIN format, required fields
2. **Role-based UI**: Edit buttons hidden for ReadOnly
3. **Search/filter**: Results update correctly

---

## Risks and Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| SSN/EIN format varies | Low | Medium | Strict regex validation, clear error messages |
| Concurrent edits | Medium | Low | Optimistic locking via version field |
| Role bypass | Low | High | Server-side enforcement, not just UI hiding |
| Migration needed for enum | Low | Low | EF migration with default values |

---

## Next Steps

1. Create updated API contract with new entity types and restore endpoint
2. Generate data-model.md with entity relationships
3. Create quickstart.md for local development setup
4. Proceed to `/speckit.tasks` for task breakdown

