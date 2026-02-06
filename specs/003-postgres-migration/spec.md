# Feature Specification: PostgreSQL Migration

**Feature Branch**: `003-postgres-migration`
**Created**: 2026-02-06
**Status**: Draft
**Input**: User description: "Migrate from SQL Server to PostgreSQL with row-level security and snake_case naming conventions for Azure hosting"

## Clarifications

### Session 2026-02-06

- Q: When a pooled connection is reused by a different tenant's request, how should the previous tenant context be handled? → A: Reset context on every connection (clear previous, then set new)
- Q: How should admin operations bypass RLS for maintenance? → A: Separate admin connection string with elevated role (app connection cannot bypass)

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Database Infrastructure Switch (Priority: P1) 🎯 MVP

Developers can start the application locally using PostgreSQL instead of SQL Server, with all existing entity configurations working correctly and data persisting between restarts.

**Why this priority**: This is the foundation for all other changes. Without the basic PostgreSQL infrastructure working, no other stories can proceed. This enables the team to develop against PostgreSQL immediately.

**Independent Test**: Start Docker containers with `./scripts/up.sh`, verify PostgreSQL is running and healthy, run the API which connects to the database successfully, and confirm the health endpoint returns OK.

**Acceptance Scenarios**:

1. **Given** a developer has Docker installed, **When** they run `./scripts/up.sh`, **Then** PostgreSQL starts and becomes healthy within 60 seconds
2. **Given** PostgreSQL is running, **When** the API starts, **Then** it connects to the database without errors
3. **Given** the API is connected, **When** the health endpoint is called, **Then** it returns a healthy status
4. **Given** data is written to the database, **When** containers are stopped and restarted, **Then** the data persists

---

### User Story 2 - Snake_case Naming Convention (Priority: P2)

All database objects (tables, columns, indexes, constraints) use snake_case naming convention, which is the PostgreSQL standard and improves compatibility with database tools and queries.

**Why this priority**: Naming conventions must be established before creating any migrations. Changing conventions after migrations exist would require significant rework.

**Independent Test**: Generate the initial migration, inspect the SQL output to verify all identifiers use snake_case format (e.g., `organization_id` not `OrganizationId`).

**Acceptance Scenarios**:

1. **Given** an entity named `Organization`, **When** the migration is generated, **Then** the table is named `organizations` (plural, lowercase)
2. **Given** a property named `CreatedAt`, **When** the migration is generated, **Then** the column is named `created_at`
3. **Given** an index on `OrganizationId`, **When** the migration is generated, **Then** the index is named `ix_table_name_organization_id`
4. **Given** a foreign key constraint, **When** the migration is generated, **Then** the constraint uses snake_case naming

---

### User Story 3 - Row-Level Security for Multi-Tenancy (Priority: P3)

The database enforces tenant isolation at the database level using PostgreSQL Row-Level Security (RLS) policies, providing defense-in-depth security that cannot be bypassed even with raw SQL access.

**Why this priority**: While the application already has query filters for tenant isolation, RLS adds a critical security layer. This is important but can be added after basic functionality works.

**Independent Test**: Connect to the database directly (bypassing the application), attempt to query data for a different tenant, and verify the RLS policy prevents access.

**Acceptance Scenarios**:

1. **Given** RLS is enabled on a tenant table, **When** a query runs without setting the tenant context, **Then** no rows are returned
2. **Given** a tenant context is set to Organization A, **When** querying clients, **Then** only Organization A's clients are visible
3. **Given** a tenant context is set to Organization A, **When** attempting to insert a client for Organization B, **Then** the insert is rejected
4. **Given** RLS policies are active, **When** using raw SQL through any database tool, **Then** tenant isolation is still enforced

---

### User Story 4 - Azure PostgreSQL Compatibility (Priority: P4)

The database configuration is compatible with Azure Database for PostgreSQL Flexible Server, enabling seamless deployment to Azure environments.

**Why this priority**: Local development is the immediate need; Azure deployment can follow once the local environment is stable.

**Independent Test**: Review connection string format and configuration options to ensure they align with Azure PostgreSQL Flexible Server requirements.

**Acceptance Scenarios**:

1. **Given** the application configuration, **When** an Azure PostgreSQL connection string is provided, **Then** the application connects successfully
2. **Given** SSL is required by Azure, **When** connecting to Azure PostgreSQL, **Then** the connection uses SSL/TLS encryption
3. **Given** Azure PostgreSQL has connection pooling recommendations, **When** configuring the connection, **Then** appropriate pool settings are applied

---

### Edge Cases

- What happens when PostgreSQL is not running but the API starts? → Application fails gracefully with clear error message
- How does the system handle connection timeouts during high load? → Connection retry with exponential backoff (3 attempts, max 30 seconds)
- What happens if RLS policy is missing on a table? → Application should verify RLS is enabled on startup in non-development environments
- How are existing PascalCase column names in code affected? → Entity Framework mapping handles translation; C# properties remain PascalCase
- What happens during migration from SQL Server to PostgreSQL for existing deployments? → This spec covers new deployments only; data migration is out of scope

## Requirements *(mandatory)*

### Functional Requirements

**Infrastructure**
- **FR-001**: System MUST use PostgreSQL 16 or later as the database engine
- **FR-002**: System MUST provide a PostgreSQL Docker container for local development
- **FR-003**: System MUST include health checks to verify database availability
- **FR-004**: System MUST persist data in named Docker volumes between container restarts

**Entity Framework Configuration**
- **FR-005**: System MUST use the Npgsql Entity Framework Core provider
- **FR-006**: System MUST automatically convert all table names to snake_case plural form
- **FR-007**: System MUST automatically convert all column names to snake_case
- **FR-008**: System MUST automatically convert all index names to snake_case with `ix_` prefix
- **FR-009**: System MUST automatically convert all foreign key constraint names to snake_case
- **FR-010**: System MUST automatically convert all primary key constraint names to snake_case

**Row-Level Security**
- **FR-011**: System MUST enable RLS on all tenant-scoped tables (clients, authorizations, transcripts, users, notifications, audit_logs)
- **FR-012**: System MUST create RLS policies that filter by `organization_id`
- **FR-013**: System MUST set the tenant context via PostgreSQL session variable (`app.organization_id`)
- **FR-014**: System MUST reset (clear then set) the tenant context on each database connection acquisition from the pool before executing queries
- **FR-015**: System MUST use a separate admin connection string with an elevated database role that bypasses RLS for maintenance operations
- **FR-015a**: System MUST ensure the standard application connection role cannot bypass RLS under any circumstances

**Connection Management**
- **FR-016**: System MUST support connection strings compatible with Azure PostgreSQL Flexible Server
- **FR-017**: System MUST implement connection retry logic with configurable attempts and delays
- **FR-018**: System MUST support SSL/TLS connections for production environments

**Development Tools**
- **FR-019**: System MUST provide pgAdmin or equivalent database management tool in Docker for development
- **FR-020**: System MUST update all lifecycle scripts to reference PostgreSQL instead of SQL Server

### Key Entities

No new entities are introduced. Existing entities are affected by naming convention changes:

- **organizations** (was Organizations): Multi-tenant anchor entity
- **users** (was Users): User accounts within organizations
- **clients** (was Clients): Tax clients managed by organizations
- **authorizations** (was Authorizations): IRS Form 8821 authorizations
- **transcripts** (was Transcripts): IRS transcript documents
- **notifications** (was Notifications): User notification records
- **audit_logs** (was AuditLogs): Security and change audit trail

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Local development environment starts with PostgreSQL in under 90 seconds
- **SC-002**: All existing entity configurations work without modification to domain entities
- **SC-003**: Generated migration SQL uses 100% snake_case identifiers with no PascalCase
- **SC-004**: RLS policies prevent cross-tenant data access in 100% of direct database queries
- **SC-005**: Application connects to Azure PostgreSQL Flexible Server without code changes (configuration only)
- **SC-006**: Database management UI (pgAdmin) is accessible for developers when using tools profile
- **SC-007**: Connection failures are retried up to 3 times before failing permanently
- **SC-008**: Zero data loss when Docker containers are stopped and restarted

## Assumptions

- PostgreSQL 16 Alpine image will be used for minimal Docker footprint
- The Npgsql.EntityFrameworkCore.PostgreSQL package supports all required EF Core 9.0 features
- RLS policies will be created via EF Core migrations using raw SQL
- The existing application-level tenant query filters will remain as a secondary safety layer
- Development uses local PostgreSQL; production will use Azure PostgreSQL Flexible Server
- No existing production data requires migration (greenfield deployment)

## Out of Scope

- Data migration from existing SQL Server deployments
- Performance benchmarking between SQL Server and PostgreSQL
- PostgreSQL-specific features beyond RLS (partitioning, advanced indexing)
- Read replicas or high-availability configuration
- Backup and restore procedures
