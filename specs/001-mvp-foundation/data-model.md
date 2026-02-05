# Data Model: IRS Transcript Analysis System - MVP Foundation

**Feature**: 001-mvp-foundation
**Date**: 2026-02-05

## Entity Relationship Diagram

```
┌─────────────────┐       ┌─────────────────┐
│  Organization   │───────│      User       │
│                 │ 1   * │                 │
└────────┬────────┘       └────────┬────────┘
         │ 1                       │ *
         │                         │
         │ *                       │ creates
┌────────┴────────┐       ┌────────┴────────┐
│     Client      │───────│  Authorization  │
│   (Taxpayer)    │ 1   * │    (8821)       │
└────────┬────────┘       └─────────────────┘
         │ 1
         │
         │ *
┌────────┴────────┐       ┌─────────────────┐
│   Transcript    │       │    AuditLog     │
│                 │       │                 │
└─────────────────┘       └─────────────────┘

┌─────────────────┐       ┌─────────────────┐
│  Notification   │       │  Organization   │
│                 │       │    Settings     │
└─────────────────┘       └─────────────────┘
```

## Entities

### Organization

Represents a service provider firm (CPA firm, tax resolution company, lender).

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| Name | string(200) | Required | Organization display name |
| Slug | string(50) | Required, Unique | URL-safe identifier |
| ContactEmail | string(254) | Required, Email | Primary contact email |
| ContactPhone | string(20) | Optional | Primary contact phone |
| Address | Address (VO) | Required | Mailing address |
| SubscriptionStatus | enum | Required | Trial, Active, Suspended, Cancelled |
| SubscriptionPlan | string(50) | Optional | Plan identifier |
| CreatedAt | DateTime | Required | Creation timestamp (UTC) |
| UpdatedAt | DateTime | Required | Last update timestamp (UTC) |
| DeletedAt | DateTime | Optional | Soft delete timestamp |

**Value Object: Address**
- Street1, Street2, City, State, PostalCode, Country

### OrganizationSettings

Configuration options for an organization.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| ESignatureProvider | enum | Required | BuiltIn, DocuSign, AdobeSign |
| AuthLinkExpirationDays | int | Required, Default: 7 | Link expiration period |
| NotificationEmailEnabled | bool | Required, Default: true | Email notifications |
| NotificationInAppEnabled | bool | Required, Default: true | In-app notifications |
| NotificationSmsEnabled | bool | Required, Default: false | SMS notifications (future) |
| DefaultTaxYearsCount | int | Required, Default: 4 | Default years on 8821 |
| Timezone | string(50) | Required | Organization timezone |

### User

A person who uses the system, belonging to one organization.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| Auth0UserId | string(128) | Required, Unique | Auth0 subject identifier |
| Email | string(254) | Required, Unique per org | User email |
| FirstName | string(100) | Required | First name |
| LastName | string(100) | Required | Last name |
| Role | enum | Required | Admin, TaxProfessional, ReadOnly |
| Status | enum | Required | Invited, Active, Suspended |
| InvitedAt | DateTime | Optional | Invitation timestamp |
| InvitedByUserId | GUID | FK, Optional | Inviting user |
| LastLoginAt | DateTime | Optional | Last login timestamp |
| CreatedAt | DateTime | Required | Creation timestamp |
| UpdatedAt | DateTime | Required | Last update timestamp |
| DeletedAt | DateTime | Optional | Soft delete timestamp |

**Indexes:**
- `IX_User_OrganizationId` (OrganizationId)
- `IX_User_Auth0UserId` (Auth0UserId) UNIQUE
- `IX_User_Email_OrganizationId` (Email, OrganizationId) UNIQUE

### Client (Taxpayer)

An individual or business whose tax information is being managed.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| ClientType | enum | Required | Individual, Business |
| FirstName | string(100) | Required if Individual | First name |
| LastName | string(100) | Required if Individual | Last name |
| BusinessName | string(200) | Required if Business | Business legal name |
| EntityType | enum | Optional if Business | LLC, SCorp, CCorp, Partnership, SoleProprietor |
| ResponsibleParty | string(200) | Optional if Business | Responsible party name |
| TaxIdentifier | EncryptedString | Required | SSN (Individual) or EIN (Business) |
| TaxIdentifierLast4 | string(4) | Required | Last 4 digits for display |
| Email | string(254) | Required | Contact email |
| Phone | string(20) | Optional | Contact phone |
| Address | Address (VO) | Required | Mailing address |
| Notes | string(2000) | Optional | Internal notes |
| CreatedByUserId | GUID | FK, Required | Creating user |
| CreatedAt | DateTime | Required | Creation timestamp |
| UpdatedAt | DateTime | Required | Last update timestamp |
| DeletedAt | DateTime | Optional | Soft delete timestamp |

**Value Object: EncryptedString**
- Stores AES-256 encrypted value
- Decryption requires Key Vault access
- Logged when decrypted

**Indexes:**
- `IX_Client_OrganizationId` (OrganizationId)
- `IX_Client_TaxIdentifierLast4` (OrganizationId, TaxIdentifierLast4)
- `IX_Client_Name` (OrganizationId, LastName, FirstName) - for Individual
- `IX_Client_BusinessName` (OrganizationId, BusinessName) - for Business

**Validation Rules:**
- SSN format: XXX-XX-XXXX (stored without dashes, encrypted)
- EIN format: XX-XXXXXXX (stored without dashes, encrypted)
- Unique TaxIdentifier per Organization (checked against encrypted hash)

### Authorization

An IRS Form 8821 authorization record.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| ClientId | GUID | FK, Required | Associated client |
| CreatedByUserId | GUID | FK, Required | Creating user |
| Status | enum | Required | See status enum below |
| TaxYears | int[] | Required, Max: 4 | Tax years covered (e.g., [2023, 2022, 2021, 2020]) |
| SignatureRequestToken | string(64) | Optional | Hashed signature link token |
| SignatureRequestExpiresAt | DateTime | Optional | Token expiration |
| SignatureData | JSON | Optional | Signature capture details |
| SignedAt | DateTime | Optional | Signature timestamp |
| SignedByIp | string(45) | Optional | Signer IP address |
| SignedByUserAgent | string(500) | Optional | Signer user agent |
| ExternalSignatureId | string(100) | Optional | DocuSign/Adobe envelope ID |
| FormBlobPath | string(500) | Optional | Signed form storage path |
| ExpirationDate | DateTime | Optional | 8821 expiration (signature + 3 years) |
| CafSubmissionDate | DateTime | Optional | Date submitted to IRS CAF |
| CafConfirmationDate | DateTime | Optional | IRS CAF confirmation date |
| CafReferenceNumber | string(50) | Optional | IRS CAF reference |
| RevokedAt | DateTime | Optional | Revocation timestamp |
| RevokedReason | string(500) | Optional | Revocation reason |
| CreatedAt | DateTime | Required | Creation timestamp |
| UpdatedAt | DateTime | Required | Last update timestamp |

**Authorization Status Enum:**
- `Draft` - 8821 created but not sent
- `PendingSignature` - Awaiting client signature
- `Signed` - Client has signed
- `SubmittedToCaf` - Submitted to IRS
- `Active` - IRS confirmed, can retrieve transcripts
- `Expired` - Past expiration date
- `Revoked` - Manually revoked

**Value Object: SignatureData (JSON)**
```json
{
  "method": "BuiltIn|DocuSign|AdobeSign",
  "typedName": "John Doe",
  "consentCheckbox": true,
  "timestamp": "2026-02-05T10:30:00Z",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0...",
  "externalEnvelopeId": "abc-123" // if third-party
}
```

**Indexes:**
- `IX_Authorization_OrganizationId` (OrganizationId)
- `IX_Authorization_ClientId` (ClientId)
- `IX_Authorization_Status` (OrganizationId, Status)
- `IX_Authorization_ExpirationDate` (ExpirationDate) WHERE Status = 'Active'

**State Transitions:**
```
Draft → PendingSignature (send request)
PendingSignature → Signed (client signs)
PendingSignature → Draft (cancel/resend)
Signed → SubmittedToCaf (submit to IRS)
SubmittedToCaf → Active (IRS confirms)
Active → Expired (expiration date passed)
Any → Revoked (manual revocation)
```

### Transcript

A stored IRS transcript document.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| ClientId | GUID | FK, Required | Associated client |
| AuthorizationId | GUID | FK, Required | Enabling authorization |
| UploadedByUserId | GUID | FK, Required | Uploading user |
| TranscriptType | enum | Required | See type enum below |
| TaxYear | int | Required | Tax year (e.g., 2023) |
| BlobPath | string(500) | Required | Azure Blob Storage path |
| FileName | string(255) | Required | Original filename |
| FileSize | long | Required | File size in bytes |
| ContentType | string(100) | Required | MIME type (application/pdf, text/xml) |
| UploadedAt | DateTime | Required | Upload timestamp |
| LastAccessedAt | DateTime | Optional | Last download/view timestamp |
| LastAccessedByUserId | GUID | FK, Optional | Last accessing user |
| CreatedAt | DateTime | Required | Creation timestamp |
| DeletedAt | DateTime | Optional | Soft delete timestamp |

**Transcript Type Enum:**
- `AccountTranscript` - Account information
- `ReturnTranscript` - Filed return data
- `RecordOfAccount` - Combined account + return
- `WageAndIncome` - W-2, 1099 information

**Blob Path Format:**
`{org_id}/{client_id}/{tax_year}/{transcript_type}_{timestamp}.{ext}`

**Indexes:**
- `IX_Transcript_OrganizationId` (OrganizationId)
- `IX_Transcript_ClientId` (ClientId)
- `IX_Transcript_AuthorizationId` (AuthorizationId)
- `IX_Transcript_TaxYear` (OrganizationId, TaxYear)

### Notification

System notifications for users.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| UserId | GUID | FK, Required | Recipient user |
| Type | enum | Required | See type enum below |
| Title | string(200) | Required | Notification title |
| Message | string(2000) | Required | Notification body |
| EntityType | string(50) | Optional | Related entity type |
| EntityId | GUID | Optional | Related entity ID |
| Channels | enum[] | Required | Delivery channels used |
| EmailSentAt | DateTime | Optional | Email send timestamp |
| ReadAt | DateTime | Optional | Read timestamp |
| CreatedAt | DateTime | Required | Creation timestamp |

**Notification Type Enum:**
- `AuthorizationSigned` - Client signed 8821
- `AuthorizationExpiring` - 8821 expiring soon
- `AuthorizationExpired` - 8821 has expired
- `TranscriptUploaded` - New transcript uploaded
- `TeamInvitation` - User invited to org
- `TeamMemberJoined` - Invitation accepted

**Indexes:**
- `IX_Notification_UserId_ReadAt` (UserId, ReadAt) - unread notifications
- `IX_Notification_CreatedAt` (OrganizationId, CreatedAt DESC)

### AuditLog

Immutable audit trail for compliance.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | GUID | PK | Unique identifier |
| OrganizationId | GUID | FK, Required | Parent organization |
| UserId | GUID | FK, Optional | Acting user (null for system) |
| Action | enum | Required | See action enum below |
| EntityType | string(50) | Required | Affected entity type |
| EntityId | GUID | Required | Affected entity ID |
| IpAddress | string(45) | Optional | Request IP address |
| UserAgent | string(500) | Optional | Request user agent |
| BeforeState | JSON | Optional | Entity state before action |
| AfterState | JSON | Optional | Entity state after action |
| Metadata | JSON | Optional | Additional context |
| Timestamp | DateTime | Required | Action timestamp (UTC) |

**Audit Action Enum:**
- `Create`, `Read`, `Update`, `Delete`
- `Login`, `Logout`, `LoginFailed`
- `SignatureRequested`, `SignatureSigned`
- `TranscriptUploaded`, `TranscriptDownloaded`
- `DataExported`, `DataDeleted`

**Redaction Rules:**
- SSN/EIN: Show "***-**-{last4}" in logs
- BeforeState/AfterState: Exclude encrypted fields

**Indexes:**
- `IX_AuditLog_OrganizationId_Timestamp` (OrganizationId, Timestamp DESC)
- `IX_AuditLog_EntityType_EntityId` (EntityType, EntityId)
- `IX_AuditLog_UserId` (UserId)
- `IX_AuditLog_Action` (Action, Timestamp DESC)

## Database Constraints

### Foreign Keys

```sql
ALTER TABLE Users ADD CONSTRAINT FK_Users_Organizations
  FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id);

ALTER TABLE Clients ADD CONSTRAINT FK_Clients_Organizations
  FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id);

ALTER TABLE Clients ADD CONSTRAINT FK_Clients_Users_CreatedBy
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id);

ALTER TABLE Authorizations ADD CONSTRAINT FK_Authorizations_Organizations
  FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id);

ALTER TABLE Authorizations ADD CONSTRAINT FK_Authorizations_Clients
  FOREIGN KEY (ClientId) REFERENCES Clients(Id);

ALTER TABLE Transcripts ADD CONSTRAINT FK_Transcripts_Organizations
  FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id);

ALTER TABLE Transcripts ADD CONSTRAINT FK_Transcripts_Clients
  FOREIGN KEY (ClientId) REFERENCES Clients(Id);

ALTER TABLE Transcripts ADD CONSTRAINT FK_Transcripts_Authorizations
  FOREIGN KEY (AuthorizationId) REFERENCES Authorizations(Id);
```

### Row-Level Security (Azure SQL)

```sql
-- Create security policy function
CREATE FUNCTION dbo.fn_TenantAccessPredicate(@OrganizationId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_result
WHERE @OrganizationId = CAST(SESSION_CONTEXT(N'OrganizationId') AS UNIQUEIDENTIFIER);

-- Apply to tenant-scoped tables
CREATE SECURITY POLICY TenantPolicy
ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(OrganizationId) ON dbo.Clients,
ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(OrganizationId) ON dbo.Authorizations,
ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(OrganizationId) ON dbo.Transcripts,
ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(OrganizationId) ON dbo.Notifications,
ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(OrganizationId) ON dbo.AuditLogs
WITH (STATE = ON);
```

## EF Core Configuration Notes

### Global Query Filters

```csharp
// Applied to all tenant-scoped entities
modelBuilder.Entity<Client>()
    .HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId);

modelBuilder.Entity<Authorization>()
    .HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId);
```

### Value Conversions

```csharp
// EncryptedString value object
modelBuilder.Entity<Client>()
    .Property(e => e.TaxIdentifier)
    .HasConversion(
        v => _encryptionService.Encrypt(v),
        v => _encryptionService.Decrypt(v));

// Enum arrays stored as JSON
modelBuilder.Entity<Authorization>()
    .Property(e => e.TaxYears)
    .HasConversion(
        v => JsonSerializer.Serialize(v),
        v => JsonSerializer.Deserialize<int[]>(v));
```

## Migration Strategy

1. **Initial Migration**: Create all tables with indexes
2. **Seed Data**: Default organization settings, admin user setup
3. **RLS Setup**: Apply row-level security policies after tables exist
4. **Encryption Keys**: Register encryption keys in Key Vault before data insert
