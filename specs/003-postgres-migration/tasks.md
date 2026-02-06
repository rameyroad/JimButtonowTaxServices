# Tasks: PostgreSQL Migration

**Input**: Design documents from `/specs/003-postgres-migration/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Integration tests for RLS validation are included per constitution requirement (Test-Driven Development).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `transcript-project/backend/src/`
- **Infrastructure**: `transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/`
- **Docker**: `transcript-project/docker/`
- **Tests**: `transcript-project/backend/tests/`

---

## Phase 1: Setup (Package Updates)

**Purpose**: Update NuGet packages for PostgreSQL support

- [x] T001 Replace Microsoft.EntityFrameworkCore.SqlServer with Npgsql.EntityFrameworkCore.PostgreSQL in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/TranscriptAnalyzer.Infrastructure.csproj
- [x] T002 [P] Add EFCore.NamingConventions package to transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/TranscriptAnalyzer.Infrastructure.csproj
- [x] T003 [P] Run dotnet restore to verify package compatibility

---

## Phase 2: Foundational (Docker Infrastructure)

**Purpose**: Replace SQL Server with PostgreSQL in Docker configuration

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Replace SQL Server service with PostgreSQL 16 in transcript-project/docker/docker-compose.yml
- [x] T005 [P] Replace SQL Server service with PostgreSQL 16 in transcript-project/docker/tilt-compose.yml
- [x] T006 Update environment variables for PostgreSQL in transcript-project/docker/.env.template
- [x] T007 Update health check script for PostgreSQL in transcript-project/docker/scripts/health-check.sh
- [x] T008 [P] Update logs script references from sqlserver to postgres in transcript-project/docker/scripts/logs.sh
- [x] T009 [P] Add pgAdmin service to docker-compose.yml tools profile in transcript-project/docker/docker-compose.yml
- [x] T010 [P] Add pgAdmin service to tilt-compose.yml in transcript-project/docker/tilt-compose.yml
- [x] T011 Replace Adminer with pgAdmin references in transcript-project/docker/README.md

**Checkpoint**: PostgreSQL Docker infrastructure ready - can start and verify container health

---

## Phase 3: User Story 1 - Database Infrastructure Switch (Priority: P1) 🎯 MVP

**Goal**: Developers can start the application locally using PostgreSQL with existing entity configurations working

**Independent Test**: Run `./scripts/up.sh`, verify PostgreSQL is healthy, run API and confirm health endpoint returns OK

### Implementation for User Story 1

- [x] T012 [US1] Update UseNpgsql configuration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T013 [US1] Add connection retry logic with EnableRetryOnFailure in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T014 [US1] Update connection string format in transcript-project/backend/src/TranscriptAnalyzer.Api/appsettings.Development.json
- [x] T015 [US1] Verify PostgreSQL container starts and becomes healthy within 60 seconds
- [x] T016 [US1] Verify API connects to PostgreSQL without errors
- [x] T017 [US1] Verify health endpoint returns OK status

**Checkpoint**: User Story 1 complete - developers can start infrastructure and run API against PostgreSQL

---

## Phase 4: User Story 2 - Snake_case Naming Convention (Priority: P2)

**Goal**: All database objects use snake_case naming convention per PostgreSQL standards

**Independent Test**: Generate migration, inspect SQL output to verify snake_case identifiers

### Implementation for User Story 2

- [x] T018 [US2] Add UseSnakeCaseNamingConvention() to DbContext options in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T019 [P] [US2] Remove explicit ToTable() calls from OrganizationConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/OrganizationConfiguration.cs
- [x] T020 [P] [US2] Remove explicit ToTable() calls from UserConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/UserConfiguration.cs
- [x] T021 [P] [US2] Remove explicit ToTable() calls from ClientConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/ClientConfiguration.cs
- [x] T022 [P] [US2] Remove explicit ToTable() calls from AuthorizationConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/AuthorizationConfiguration.cs
- [x] T023 [P] [US2] Remove explicit ToTable() calls from TranscriptConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/TranscriptConfiguration.cs
- [x] T024 [P] [US2] Remove explicit ToTable() calls from NotificationConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs
- [x] T025 [P] [US2] Remove explicit ToTable() calls from AuditLogConfiguration in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs
- [x] T026 [US2] Generate Initial migration with snake_case naming in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T027 [US2] Verify migration SQL uses snake_case for all tables, columns, indexes, and constraints

**Checkpoint**: User Story 2 complete - all database objects use snake_case naming

---

## Phase 5: User Story 3 - Row-Level Security (Priority: P3)

**Goal**: Database enforces tenant isolation using PostgreSQL RLS policies

**Independent Test**: Connect directly to database, set tenant context, verify only that tenant's data is visible

### Tests for User Story 3

- [x] T028 [P] [US3] Create RLS integration test project in transcript-project/backend/tests/TranscriptAnalyzer.Infrastructure.Tests/TranscriptAnalyzer.Infrastructure.Tests.csproj
- [x] T029 [P] [US3] Write integration test: query without tenant context returns no rows in transcript-project/backend/tests/TranscriptAnalyzer.Infrastructure.Tests/RlsIntegrationTests.cs
- [x] T030 [P] [US3] Write integration test: query with tenant A context returns only tenant A data in transcript-project/backend/tests/TranscriptAnalyzer.Infrastructure.Tests/RlsIntegrationTests.cs
- [x] T031 [P] [US3] Write integration test: insert with wrong tenant context is rejected in transcript-project/backend/tests/TranscriptAnalyzer.Infrastructure.Tests/RlsIntegrationTests.cs

### Implementation for User Story 3

- [x] T032 [US3] Create TenantConnectionInterceptor class in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/TenantConnectionInterceptor.cs
- [x] T033 [US3] Implement ConnectionOpeningAsync to reset and set tenant context in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/TenantConnectionInterceptor.cs
- [x] T034 [US3] Register TenantConnectionInterceptor in DependencyInjection in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T035 [US3] Create migration for database roles (app_user, app_admin) in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T036 [US3] Add RLS policy for users table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T037 [P] [US3] Add RLS policy for clients table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T038 [P] [US3] Add RLS policy for authorizations table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T039 [P] [US3] Add RLS policy for transcripts table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T040 [P] [US3] Add RLS policy for notifications table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T041 [P] [US3] Add RLS policy for audit_logs table in migration transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [x] T042 [US3] Add AdminConnection string support in DependencyInjection for RLS bypass in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T043 [US3] Run integration tests to verify RLS policies work correctly
- [x] T044 [US3] Verify admin connection can bypass RLS for maintenance operations

**Checkpoint**: User Story 3 complete - RLS enforces tenant isolation at database level

---

## Phase 6: User Story 4 - Azure PostgreSQL Compatibility (Priority: P4)

**Goal**: Configuration supports Azure Database for PostgreSQL Flexible Server

**Independent Test**: Verify connection string format and SSL settings align with Azure requirements

### Implementation for User Story 4

- [x] T045 [US4] Add SSL configuration support in DependencyInjection in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T046 [US4] Add connection pooling configuration (MaxPoolSize, MinPoolSize) in transcript-project/backend/src/TranscriptAnalyzer.Infrastructure/DependencyInjection.cs
- [x] T047 [US4] Document Azure connection string format in transcript-project/docker/.env.template
- [x] T048 [US4] Add appsettings.Production.json with Azure PostgreSQL template in transcript-project/backend/src/TranscriptAnalyzer.Api/appsettings.Production.json
- [x] T049 [US4] Verify application works with SSLMode=Require connection string

**Checkpoint**: User Story 4 complete - application is Azure PostgreSQL ready

---

## Phase 7: Polish & Documentation

**Purpose**: Documentation updates and final validation

- [x] T050 [P] Update quickstart.md with PostgreSQL commands in specs/003-postgres-migration/quickstart.md
- [x] T051 [P] Update docker README with PostgreSQL information in transcript-project/docker/README.md
- [x] T052 Run quickstart.md validation - verify all documented commands work
- [x] T053 Verify all success criteria from spec.md are met

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational phase completion
  - US1 (P1): First priority - MVP
  - US2 (P2): Depends on US1 (needs working PostgreSQL to test naming)
  - US3 (P3): Depends on US2 (needs initial migration before adding RLS)
  - US4 (P4): Independent of US2/US3 but should verify after US1
- **Polish (Phase 7)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - MVP!
- **User Story 2 (P2)**: Builds on US1 - needs PostgreSQL running to generate migrations
- **User Story 3 (P3)**: Builds on US2 - RLS added via additional migration
- **User Story 4 (P4)**: Can test after US1 - configuration changes only

### Within Each User Story

- Tests written first (US3 only, per TDD requirement)
- Configuration before implementation
- Migrations after code changes
- Verification tasks after implementation

### Parallel Opportunities

**Phase 1 (Setup)**:
```bash
# These can run in parallel:
T002 [P] Add EFCore.NamingConventions package
T003 [P] Run dotnet restore
```

**Phase 2 (Foundational)**:
```bash
# These can run in parallel:
T005 [P] Update tilt-compose.yml
T008 [P] Update logs script
T009 [P] Add pgAdmin to docker-compose
T010 [P] Add pgAdmin to tilt-compose
```

**Phase 4 (US2)**:
```bash
# Remove ToTable calls in parallel (different files):
T019 [P] OrganizationConfiguration
T020 [P] UserConfiguration
T021 [P] ClientConfiguration
T022 [P] AuthorizationConfiguration
T023 [P] TranscriptConfiguration
T024 [P] NotificationConfiguration
T025 [P] AuditLogConfiguration
```

**Phase 5 (US3)**:
```bash
# Write tests in parallel:
T028 [P] Create test project
T029 [P] Test: no context = no rows
T030 [P] Test: context A = A data only
T031 [P] Test: wrong tenant insert rejected

# Add RLS policies in parallel (same migration file, different tables):
T037 [P] clients RLS
T038 [P] authorizations RLS
T039 [P] transcripts RLS
T040 [P] notifications RLS
T041 [P] audit_logs RLS
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (Docker infrastructure)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test PostgreSQL connection, verify health endpoint
5. Developers can now run API against PostgreSQL

### Incremental Delivery

1. Complete Setup + Foundational → PostgreSQL Docker ready
2. Add User Story 1 → Test independently → PostgreSQL working (MVP!)
3. Add User Story 2 → Generate migration → Snake_case verified
4. Add User Story 3 → Run RLS tests → Tenant isolation enforced
5. Add User Story 4 → Configuration only → Azure ready

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- US2 depends on US1 because migration generation requires running PostgreSQL
- US3 depends on US2 because RLS is added to existing migration
- US4 is configuration-only and can be verified after US1
- Reference research.md for implementation patterns
- Reference data-model.md for table/column names
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
