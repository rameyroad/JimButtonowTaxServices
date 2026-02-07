# Data Model: Client Management

**Feature**: 004-client-management
**Date**: 2026-02-06
**Spec**: [spec.md](./spec.md)

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                                 ORGANIZATION                                     │
│  (Multi-tenant root - RLS enforced)                                             │
└─────────────────────────────────────────────────────────────────────────────────┘
         │
         │ 1:N
         ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                                    CLIENT                                        │
├─────────────────────────────────────────────────────────────────────────────────┤
│  id: UUID (PK)                                                                   │
│  organization_id: UUID (FK → Organization, RLS)                                 │
│  client_type: ClientType (Individual | Business)                                │
│  ─────────── Individual Fields ───────────                                       │
│  first_name: string (null if Business)                                          │
│  last_name: string (null if Business)                                           │
│  ─────────── Business Fields ─────────────                                       │
│  business_name: string (null if Individual)                                     │
│  entity_type: BusinessEntityType (null if Individual)                           │
│  responsible_party: string (nullable)                                           │
│  ─────────── Common Fields ───────────────                                       │
│  tax_identifier_encrypted: string (AES-256 encrypted SSN/EIN)                   │
│  tax_identifier_last4: string (4 chars, unencrypted for display/search)         │
│  email: string                                                                   │
│  phone: string (nullable)                                                        │
│  notes: text (nullable)                                                          │
│  ─────────── Address (Owned Entity) ──────                                       │
│  address_street1: string                                                         │
│  address_street2: string (nullable)                                              │
│  address_city: string                                                            │
│  address_state: string                                                           │
│  address_postal_code: string                                                     │
│  address_country: string (default: "US")                                         │
│  ─────────── Audit Fields ────────────────                                       │
│  created_at: timestamp                                                           │
│  created_by_id: UUID (FK → User)                                                │
│  updated_at: timestamp                                                           │
│  deleted_at: timestamp (nullable, soft delete)                                   │
│  version: int (optimistic concurrency)                                           │
└─────────────────────────────────────────────────────────────────────────────────┘
         │
         │ 1:N
         ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                                 AUDIT_LOG                                        │
├─────────────────────────────────────────────────────────────────────────────────┤
│  id: UUID (PK)                                                                   │
│  entity_type: string ("Client")                                                  │
│  entity_id: UUID                                                                 │
│  action: string (Create | Update | Archive | Restore)                           │
│  performed_by_id: UUID (FK → User)                                              │
│  performed_at: timestamp                                                         │
│  changes: jsonb (field-level change tracking, no PII)                           │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## Enumerations

### ClientType

| Value | Code | Description |
|-------|------|-------------|
| Individual | 0 | Person with Social Security Number (SSN) |
| Business | 1 | Entity with Employer Identification Number (EIN) |

### BusinessEntityType

| Value | Code | Description |
|-------|------|-------------|
| SoleProprietor | 0 | Individual operating a business |
| Partnership | 1 | Two or more persons in business |
| SCorp | 2 | S-Corporation |
| CCorp | 3 | C-Corporation |
| LLC | 4 | Limited Liability Company |
| NonProfit | 5 | 501(c) non-profit organization |
| Trust | 6 | Trust entity |
| Estate | 7 | Estate entity |

### UserRole (Existing)

| Value | Code | Permissions |
|-------|------|-------------|
| ReadOnly | 0 | View clients only |
| TaxProfessional | 1 | View, create, edit clients |
| Admin | 2 | Full access including archive/restore |

## Field Specifications

### Client Entity

| Field | Type | Required | Constraints | Notes |
|-------|------|----------|-------------|-------|
| id | UUID | Yes | PK, auto-generated | |
| organization_id | UUID | Yes | FK, RLS enforced | Set from TenantContext |
| client_type | enum | Yes | Individual, Business | Determines which fields are relevant |
| first_name | string | Individual only | max 100 chars | |
| last_name | string | Individual only | max 100 chars | |
| business_name | string | Business only | max 200 chars | |
| entity_type | enum | Business only | See BusinessEntityType | |
| responsible_party | string | No | max 200 chars | Business contact person |
| tax_identifier_encrypted | string | Yes | max 500 chars | AES-256 encrypted |
| tax_identifier_last4 | string | Yes | exactly 4 chars | Indexed for search |
| email | string | Yes | valid email, max 254 | |
| phone | string | No | max 20 chars | |
| notes | text | No | max 2000 chars | |
| address_* | various | Yes | See Address value object | |
| created_at | timestamp | Yes | auto-set | |
| created_by_id | UUID | Yes | FK → User | |
| updated_at | timestamp | Yes | auto-updated | |
| deleted_at | timestamp | No | null = active | Soft delete marker |
| version | int | Yes | default 1 | Optimistic concurrency |

### Address Value Object

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| street1 | string | Yes | max 200 chars |
| street2 | string | No | max 200 chars |
| city | string | Yes | max 100 chars |
| state | string | Yes | max 50 chars |
| postal_code | string | Yes | max 20 chars |
| country | string | Yes | max 50 chars, default "US" |

## Validation Rules

### SSN Format (Individual)

```regex
^\d{3}-\d{2}-\d{4}$
```

Example: `123-45-6789`

### EIN Format (Business)

```regex
^\d{2}-\d{7}$
```

Example: `12-3456789`

### Conditional Requirements

| Condition | Required Fields |
|-----------|-----------------|
| client_type = Individual | first_name, last_name |
| client_type = Business | business_name, entity_type |

## Indexes

| Name | Columns | Type | Purpose |
|------|---------|------|---------|
| ix_clients_organization | organization_id | B-tree | RLS performance |
| ix_clients_org_taxid_last4 | organization_id, tax_identifier_last4 | B-tree | Duplicate detection, search |
| ix_clients_org_deleted | organization_id, deleted_at | B-tree | Active client filtering |
| ix_clients_name_search | organization_id, (display_name pattern) | GIN | Full-text search |

## Row-Level Security

```sql
-- Enable RLS on clients table
ALTER TABLE clients ENABLE ROW LEVEL SECURITY;

-- Policy: Users can only access clients in their organization
CREATE POLICY tenant_isolation ON clients
    USING (organization_id = current_setting('app.organization_id')::uuid);
```

## PII Handling

### Encryption

| Field | Storage | Display | API Response |
|-------|---------|---------|--------------|
| SSN/EIN | AES-256 encrypted | ***-**-1234 | taxIdentifierLast4 only |
| Email | Plain text | Full value | Full value |
| Phone | Plain text | Full value | Full value |
| Address | Plain text | Full value | Full value |
| Notes | Plain text | Full value | Full value |

### Audit Trail for PII Changes

When tax_identifier is updated:
1. Audit log records: "tax_identifier changed" (no values)
2. New value encrypted with fresh IV
3. tax_identifier_last4 updated accordingly

## Migration Notes

### BusinessEntityType Extension

```sql
-- Add new enum values (PostgreSQL)
ALTER TYPE business_entity_type ADD VALUE 'NonProfit';
ALTER TYPE business_entity_type ADD VALUE 'Trust';
ALTER TYPE business_entity_type ADD VALUE 'Estate';
```

### Version Field Addition

```sql
-- Add optimistic concurrency support
ALTER TABLE clients ADD COLUMN version INT NOT NULL DEFAULT 1;
```
