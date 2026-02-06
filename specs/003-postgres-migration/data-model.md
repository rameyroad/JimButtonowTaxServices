# Data Model: PostgreSQL Migration

**Date**: 2026-02-06
**Branch**: `003-postgres-migration`

## Overview

This document maps existing entities to their PostgreSQL snake_case equivalents. No schema changes are introduced; this is a naming convention transformation only.

## Naming Convention Rules

| .NET Convention | PostgreSQL Convention | Example |
|-----------------|----------------------|---------|
| PascalCase class | snake_case table (plural) | `Organization` → `organizations` |
| PascalCase property | snake_case column | `CreatedAt` → `created_at` |
| `IX_Table_Column` | `ix_table_column` | `IX_Users_Email` → `ix_users_email` |
| `PK_Table` | `pk_table` | `PK_Organizations` → `pk_organizations` |
| `FK_Child_Parent_Id` | `fk_child_parent_id` | `FK_Users_OrganizationId` → `fk_users_organization_id` |

## Entity Mappings

### organizations

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| Name | name | varchar(200) | NOT NULL |
| Slug | slug | varchar(50) | UNIQUE, NOT NULL |
| ContactEmail | contact_email | varchar(254) | NOT NULL |
| ContactPhone | contact_phone | varchar(20) | |
| SubscriptionStatus | subscription_status | varchar(20) | NOT NULL, DEFAULT 'Trial' |
| Address_Street1 | address_street1 | varchar(200) | |
| Address_Street2 | address_street2 | varchar(200) | |
| Address_City | address_city | varchar(100) | |
| Address_State | address_state | varchar(50) | |
| Address_PostalCode | address_postal_code | varchar(20) | |
| Address_Country | address_country | varchar(100) | |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |
| DeletedAt | deleted_at | timestamptz | |

**Indexes:**
- `ix_organizations_slug` UNIQUE on `slug`

**RLS:** Not applied (anchor entity)

---

### users

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| Auth0UserId | auth0_user_id | varchar(128) | UNIQUE |
| Email | email | varchar(254) | NOT NULL |
| FirstName | first_name | varchar(100) | NOT NULL |
| LastName | last_name | varchar(100) | NOT NULL |
| Role | role | varchar(20) | NOT NULL |
| Status | status | varchar(20) | NOT NULL, DEFAULT 'Active' |
| InvitedByUserId | invited_by_user_id | uuid | FK |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |
| DeletedAt | deleted_at | timestamptz | |

**Indexes:**
- `ix_users_organization_id` on `organization_id`
- `ix_users_auth0_user_id` UNIQUE on `auth0_user_id`
- `ix_users_email_organization_id` UNIQUE on `(email, organization_id)`

**RLS:** Applied - filtered by `organization_id`

---

### clients

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| ClientType | client_type | varchar(20) | NOT NULL |
| FirstName | first_name | varchar(100) | |
| LastName | last_name | varchar(100) | |
| BusinessName | business_name | varchar(200) | |
| EntityType | entity_type | varchar(30) | |
| TaxIdentifier_Encrypted | tax_identifier_encrypted | varchar(500) | |
| TaxIdentifierLast4 | tax_identifier_last4 | varchar(4) | |
| Email | email | varchar(254) | NOT NULL |
| Phone | phone | varchar(20) | |
| Address_* | address_* | (see organizations) | |
| Notes | notes | varchar(2000) | |
| CreatedByUserId | created_by_user_id | uuid | FK, NOT NULL |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |
| DeletedAt | deleted_at | timestamptz | |

**Indexes:**
- `ix_clients_organization_id` on `organization_id`
- `ix_clients_name` on `(last_name, first_name)`
- `ix_clients_business_name` on `business_name`
- `ix_clients_tax_identifier_last4` on `tax_identifier_last4`

**RLS:** Applied - filtered by `organization_id`

---

### authorizations

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| ClientId | client_id | uuid | FK, NOT NULL |
| CreatedByUserId | created_by_user_id | uuid | FK, NOT NULL |
| Status | status | varchar(20) | NOT NULL, DEFAULT 'Draft' |
| TaxYears | tax_years | jsonb | |
| SignatureRequestToken | signature_request_token | varchar(100) | |
| SignatureData | signature_data | text | |
| SignedAt | signed_at | timestamptz | |
| SignedByIp | signed_by_ip | varchar(45) | |
| SignedByUserAgent | signed_by_user_agent | varchar(500) | |
| ExternalSignatureId | external_signature_id | varchar(100) | |
| FormBlobPath | form_blob_path | varchar(500) | |
| ExpirationDate | expiration_date | timestamptz | |
| CafSubmissionDate | caf_submission_date | timestamptz | |
| CafConfirmationDate | caf_confirmation_date | timestamptz | |
| CafReferenceNumber | caf_reference_number | varchar(50) | |
| RevokedAt | revoked_at | timestamptz | |
| RevokedReason | revoked_reason | varchar(500) | |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |
| DeletedAt | deleted_at | timestamptz | |

**Indexes:**
- `ix_authorizations_organization_id` on `organization_id`
- `ix_authorizations_client_id` on `client_id`
- `ix_authorizations_status` on `(organization_id, status)`
- `ix_authorizations_expiration_date` PARTIAL on `expiration_date` WHERE `status = 'Active'`

**RLS:** Applied - filtered by `organization_id`

---

### transcripts

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| ClientId | client_id | uuid | FK, NOT NULL |
| AuthorizationId | authorization_id | uuid | FK, NOT NULL |
| UploadedByUserId | uploaded_by_user_id | uuid | FK, NOT NULL |
| TranscriptType | transcript_type | varchar(30) | NOT NULL |
| TaxYear | tax_year | integer | NOT NULL |
| BlobPath | blob_path | varchar(500) | NOT NULL |
| FileName | file_name | varchar(255) | NOT NULL |
| FileSize | file_size | bigint | NOT NULL |
| ContentType | content_type | varchar(100) | NOT NULL |
| UploadedAt | uploaded_at | timestamptz | NOT NULL |
| LastAccessedAt | last_accessed_at | timestamptz | |
| LastAccessedByUserId | last_accessed_by_user_id | uuid | FK |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |
| DeletedAt | deleted_at | timestamptz | |

**Indexes:**
- `ix_transcripts_organization_id` on `organization_id`
- `ix_transcripts_client_id` on `client_id`
- `ix_transcripts_authorization_id` on `authorization_id`
- `ix_transcripts_tax_year` on `(organization_id, tax_year)`

**RLS:** Applied - filtered by `organization_id`

---

### notifications

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| UserId | user_id | uuid | FK, NOT NULL |
| Type | type | varchar(30) | NOT NULL |
| Title | title | varchar(200) | NOT NULL |
| Message | message | varchar(2000) | NOT NULL |
| EntityType | entity_type | varchar(50) | |
| EntityId | entity_id | uuid | |
| Channels | channels | jsonb | |
| EmailSentAt | email_sent_at | timestamptz | |
| ReadAt | read_at | timestamptz | |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |

**Indexes:**
- `ix_notifications_user_id_read_at` on `(user_id, read_at)`
- `ix_notifications_created_at` on `(organization_id, created_at DESC)`

**RLS:** Applied - filtered by `organization_id`

---

### audit_logs

| C# Property | PostgreSQL Column | Type | Constraints |
|-------------|-------------------|------|-------------|
| Id | id | uuid | PK |
| OrganizationId | organization_id | uuid | FK, NOT NULL |
| UserId | user_id | uuid | FK |
| Action | action | varchar(30) | NOT NULL |
| EntityType | entity_type | varchar(50) | NOT NULL |
| EntityId | entity_id | uuid | NOT NULL |
| IpAddress | ip_address | varchar(45) | |
| UserAgent | user_agent | varchar(500) | |
| BeforeState | before_state | jsonb | |
| AfterState | after_state | jsonb | |
| Metadata | metadata | jsonb | |
| Timestamp | timestamp | timestamptz | NOT NULL |
| CreatedAt | created_at | timestamptz | NOT NULL |
| UpdatedAt | updated_at | timestamptz | NOT NULL |

**Indexes:**
- `ix_audit_logs_organization_id_timestamp` on `(organization_id, timestamp DESC)`
- `ix_audit_logs_user_id` on `user_id`
- `ix_audit_logs_action` on `(action, timestamp DESC)`
- `ix_audit_logs_entity_type_entity_id` on `(entity_type, entity_id)`

**RLS:** Applied - filtered by `organization_id`

---

## RLS Policy Template

Applied to each tenant-scoped table:

```sql
ALTER TABLE {table_name} ENABLE ROW LEVEL SECURITY;

CREATE POLICY tenant_isolation ON {table_name}
    FOR ALL
    TO app_user
    USING (organization_id = NULLIF(current_setting('app.organization_id', TRUE), '')::uuid);

-- For INSERT/UPDATE, also check the value being written
CREATE POLICY tenant_write_check ON {table_name}
    FOR INSERT
    TO app_user
    WITH CHECK (organization_id = NULLIF(current_setting('app.organization_id', TRUE), '')::uuid);
```

## Database Roles

```sql
-- Application role (RLS enforced)
CREATE ROLE app_user NOLOGIN;
GRANT USAGE ON SCHEMA public TO app_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_user;

-- Admin role (RLS bypass)
CREATE ROLE app_admin NOLOGIN BYPASSRLS;
GRANT ALL ON SCHEMA public TO app_admin;
GRANT ALL ON ALL TABLES IN SCHEMA public TO app_admin;

-- Login roles
CREATE USER transcript_app WITH PASSWORD 'xxx' IN ROLE app_user;
CREATE USER transcript_admin WITH PASSWORD 'xxx' IN ROLE app_admin;
```
