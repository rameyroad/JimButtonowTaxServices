# Tasks: IRS Transcript Analysis System - MVP Foundation

**Input**: Design documents from `/specs/001-mvp-foundation/`
**Prerequisites**: plan.md ✓, spec.md ✓, research.md ✓, data-model.md ✓, contracts/ ✓

**Tests**: Included per constitution (TDD required - 80% coverage target)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/TranscriptAnalyzer.{Layer}/`, `backend/tests/`
- **Frontend**: `frontend/src/`, `frontend/tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create backend solution structure with four projects per plan.md in backend/TranscriptAnalyzer.sln
- [ ] T002 [P] Initialize TranscriptAnalyzer.Domain project in backend/src/TranscriptAnalyzer.Domain/
- [ ] T003 [P] Initialize TranscriptAnalyzer.Application project in backend/src/TranscriptAnalyzer.Application/
- [ ] T004 [P] Initialize TranscriptAnalyzer.Infrastructure project in backend/src/TranscriptAnalyzer.Infrastructure/
- [ ] T005 [P] Initialize TranscriptAnalyzer.Api project in backend/src/TranscriptAnalyzer.Api/
- [ ] T006 [P] Create test projects structure in backend/tests/ (Domain.Tests, Application.Tests, Infrastructure.Tests, Api.Tests, Contract.Tests)
- [ ] T007 Create Next.js 14 frontend project with TypeScript in frontend/
- [ ] T008 [P] Copy and adapt CargoMax UI components to frontend/src/components/ui/
- [ ] T009 [P] Configure ESLint and Prettier for frontend in frontend/.eslintrc.js and frontend/.prettierrc
- [ ] T010 [P] Configure dotnet format and analyzers for backend in backend/Directory.Build.props
- [ ] T011 Create docker-compose.yml with SQL Server, Redis, and Azurite at repository root
- [ ] T012 [P] Create environment configuration templates (backend/.env.example, frontend/.env.local.example)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Layer Foundation

- [ ] T013 Create base Entity class with Id, CreatedAt, UpdatedAt in backend/src/TranscriptAnalyzer.Domain/Common/BaseEntity.cs
- [ ] T014 [P] Create Address value object in backend/src/TranscriptAnalyzer.Domain/ValueObjects/Address.cs
- [ ] T015 [P] Create EncryptedString value object in backend/src/TranscriptAnalyzer.Domain/ValueObjects/EncryptedString.cs
- [ ] T016 [P] Create domain enums (UserRole, UserStatus, ClientType, EntityType, AuthorizationStatus, TranscriptType, NotificationType) in backend/src/TranscriptAnalyzer.Domain/Enums/
- [ ] T017 Create Organization entity in backend/src/TranscriptAnalyzer.Domain/Entities/Organization.cs
- [ ] T018 [P] Create OrganizationSettings entity in backend/src/TranscriptAnalyzer.Domain/Entities/OrganizationSettings.cs
- [ ] T019 Create User entity in backend/src/TranscriptAnalyzer.Domain/Entities/User.cs
- [ ] T020 Create AuditLog entity in backend/src/TranscriptAnalyzer.Domain/Entities/AuditLog.cs
- [ ] T021 [P] Create domain interfaces (IRepository, IUnitOfWork) in backend/src/TranscriptAnalyzer.Domain/Interfaces/

### Infrastructure Foundation

- [ ] T022 Create ApplicationDbContext with entity configurations in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/ApplicationDbContext.cs
- [ ] T023 Implement TenantContext for multi-tenancy in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/TenantContext.cs
- [ ] T024 [P] Create EF Core configuration for Organization in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/OrganizationConfiguration.cs
- [ ] T025 [P] Create EF Core configuration for User in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/UserConfiguration.cs
- [ ] T026 [P] Create EF Core configuration for AuditLog in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs
- [ ] T027 Create initial database migration in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [ ] T028 Implement IEncryptionService with Azure Key Vault in backend/src/TranscriptAnalyzer.Infrastructure/Security/EncryptionService.cs
- [ ] T029 [P] Implement IBlobStorageService for Azure Blob Storage in backend/src/TranscriptAnalyzer.Infrastructure/Storage/BlobStorageService.cs
- [ ] T030 [P] Implement audit logging interceptor in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs

### Application Layer Foundation

- [ ] T031 Configure MediatR and AutoMapper in backend/src/TranscriptAnalyzer.Application/DependencyInjection.cs
- [ ] T032 [P] Create common DTOs (PaginatedList, Result) in backend/src/TranscriptAnalyzer.Application/Common/
- [ ] T033 [P] Create validation behaviors for MediatR pipeline in backend/src/TranscriptAnalyzer.Application/Common/Behaviors/ValidationBehavior.cs
- [ ] T034 [P] Create logging behavior for MediatR pipeline in backend/src/TranscriptAnalyzer.Application/Common/Behaviors/LoggingBehavior.cs

### API Layer Foundation

- [ ] T035 Configure Minimal API with OpenAPI/Swagger in backend/src/TranscriptAnalyzer.Api/Program.cs
- [ ] T036 Implement JWT authentication middleware with Auth0 in backend/src/TranscriptAnalyzer.Api/Middleware/AuthenticationMiddleware.cs
- [ ] T037 [P] Implement tenant resolution middleware in backend/src/TranscriptAnalyzer.Api/Middleware/TenantMiddleware.cs
- [ ] T038 [P] Implement ProblemDetails exception handler in backend/src/TranscriptAnalyzer.Api/Middleware/ExceptionMiddleware.cs
- [ ] T039 [P] Configure CORS policies per environment in backend/src/TranscriptAnalyzer.Api/Configuration/CorsConfiguration.cs

### Frontend Foundation

- [ ] T040 Configure Auth0 provider and protected routes in frontend/src/app/(auth)/
- [ ] T041 [P] Create Redux store with RTK Query setup in frontend/src/store/store.ts
- [ ] T042 [P] Create base API slice with auth headers in frontend/src/lib/api/baseApi.ts
- [ ] T043 [P] Create shared layout with sidebar navigation in frontend/src/app/(dashboard)/layout.tsx
- [ ] T044 [P] Create TypeScript types from OpenAPI contracts in frontend/src/types/

### Foundation Tests

- [ ] T045 [P] Unit tests for EncryptedString value object in backend/tests/TranscriptAnalyzer.Domain.Tests/ValueObjects/EncryptedStringTests.cs
- [ ] T046 [P] Unit tests for Address value object in backend/tests/TranscriptAnalyzer.Domain.Tests/ValueObjects/AddressTests.cs
- [ ] T047 [P] Integration tests for TenantContext in backend/tests/TranscriptAnalyzer.Infrastructure.Tests/Persistence/TenantContextTests.cs
- [ ] T048 [P] Integration tests for EncryptionService in backend/tests/TranscriptAnalyzer.Infrastructure.Tests/Security/EncryptionServiceTests.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Tax Professional Onboards New Client (Priority: P1) 🎯 MVP

**Goal**: Enable tax professionals to create and manage client records with encrypted PII

**Independent Test**: Create a new client record and verify it appears in the client list with all required fields properly stored and encrypted

### Tests for User Story 1 (TDD - Write First)

- [ ] T049 [P] [US1] Contract tests for clients-api.yaml endpoints in backend/tests/TranscriptAnalyzer.Contract.Tests/ClientsApiTests.cs
- [ ] T050 [P] [US1] Unit tests for Client entity validation in backend/tests/TranscriptAnalyzer.Domain.Tests/Entities/ClientTests.cs
- [ ] T051 [P] [US1] Unit tests for CreateClientCommand handler in backend/tests/TranscriptAnalyzer.Application.Tests/Clients/CreateClientCommandTests.cs
- [ ] T052 [P] [US1] Unit tests for GetClientsQuery handler in backend/tests/TranscriptAnalyzer.Application.Tests/Clients/GetClientsQueryTests.cs

### Backend Implementation for User Story 1

- [ ] T053 [US1] Create Client entity with validation in backend/src/TranscriptAnalyzer.Domain/Entities/Client.cs
- [ ] T054 [US1] Create EF Core configuration for Client in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/ClientConfiguration.cs
- [ ] T055 [US1] Add Client migration to database in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [ ] T056 [P] [US1] Create ClientDto and mappings in backend/src/TranscriptAnalyzer.Application/Clients/ClientDto.cs
- [ ] T057 [P] [US1] Create CreateClientCommand with FluentValidation in backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/
- [ ] T058 [P] [US1] Create UpdateClientCommand in backend/src/TranscriptAnalyzer.Application/Clients/Commands/UpdateClient/
- [ ] T059 [P] [US1] Create DeleteClientCommand in backend/src/TranscriptAnalyzer.Application/Clients/Commands/DeleteClient/
- [ ] T060 [P] [US1] Create GetClientQuery in backend/src/TranscriptAnalyzer.Application/Clients/Queries/GetClient/
- [ ] T061 [P] [US1] Create GetClientsQuery with pagination in backend/src/TranscriptAnalyzer.Application/Clients/Queries/GetClients/
- [ ] T062 [US1] Implement clients API endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs

### Frontend Implementation for User Story 1

- [ ] T063 [P] [US1] Create clients API slice in frontend/src/lib/api/clientsApi.ts
- [ ] T064 [P] [US1] Create ClientForm component with validation in frontend/src/components/clients/ClientForm.tsx
- [ ] T065 [P] [US1] Create ClientList component with data table in frontend/src/components/clients/ClientList.tsx
- [ ] T066 [P] [US1] Create ClientDetail component in frontend/src/components/clients/ClientDetail.tsx
- [ ] T067 [US1] Create clients list page in frontend/src/app/(dashboard)/clients/page.tsx
- [ ] T068 [US1] Create add client page in frontend/src/app/(dashboard)/clients/new/page.tsx
- [ ] T069 [US1] Create client detail page in frontend/src/app/(dashboard)/clients/[id]/page.tsx
- [ ] T070 [US1] Create edit client page in frontend/src/app/(dashboard)/clients/[id]/edit/page.tsx

### Frontend Tests for User Story 1

- [ ] T071 [P] [US1] Unit tests for ClientForm in frontend/tests/unit/components/clients/ClientForm.test.tsx
- [ ] T072 [P] [US1] E2E test for client creation flow in frontend/tests/e2e/clients.spec.ts

**Checkpoint**: User Story 1 complete - tax professionals can onboard clients with encrypted PII

---

## Phase 4: User Story 2 - Tax Professional Initiates 8821 Authorization (Priority: P2)

**Goal**: Enable tax professionals to create and send Form 8821 authorization requests

**Independent Test**: Generate an 8821 form for an existing client and verify the form is correctly populated and email sent

### Tests for User Story 2 (TDD - Write First)

- [ ] T073 [P] [US2] Contract tests for authorizations-api.yaml endpoints in backend/tests/TranscriptAnalyzer.Contract.Tests/AuthorizationsApiTests.cs
- [ ] T074 [P] [US2] Unit tests for Authorization entity in backend/tests/TranscriptAnalyzer.Domain.Tests/Entities/AuthorizationTests.cs
- [ ] T075 [P] [US2] Unit tests for CreateAuthorizationCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Authorizations/CreateAuthorizationCommandTests.cs
- [ ] T076 [P] [US2] Unit tests for SendAuthorizationCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Authorizations/SendAuthorizationCommandTests.cs

### Backend Implementation for User Story 2

- [ ] T077 [US2] Create Authorization entity with state machine in backend/src/TranscriptAnalyzer.Domain/Entities/Authorization.cs
- [ ] T078 [US2] Create EF Core configuration for Authorization in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/AuthorizationConfiguration.cs
- [ ] T079 [US2] Add Authorization migration in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [ ] T080 [P] [US2] Implement IEmailService interface and SendGrid implementation in backend/src/TranscriptAnalyzer.Infrastructure/Notifications/EmailService.cs
- [ ] T081 [P] [US2] Create Form8821Generator service in backend/src/TranscriptAnalyzer.Infrastructure/Documents/Form8821Generator.cs
- [ ] T082 [P] [US2] Create SignatureTokenService for secure link generation in backend/src/TranscriptAnalyzer.Infrastructure/Security/SignatureTokenService.cs
- [ ] T083 [P] [US2] Create AuthorizationDto and mappings in backend/src/TranscriptAnalyzer.Application/Authorizations/AuthorizationDto.cs
- [ ] T084 [P] [US2] Create CreateAuthorizationCommand in backend/src/TranscriptAnalyzer.Application/Authorizations/Commands/CreateAuthorization/
- [ ] T085 [P] [US2] Create SendAuthorizationCommand in backend/src/TranscriptAnalyzer.Application/Authorizations/Commands/SendAuthorization/
- [ ] T086 [P] [US2] Create ResendAuthorizationCommand in backend/src/TranscriptAnalyzer.Application/Authorizations/Commands/ResendAuthorization/
- [ ] T087 [P] [US2] Create GetAuthorizationsQuery in backend/src/TranscriptAnalyzer.Application/Authorizations/Queries/GetAuthorizations/
- [ ] T088 [US2] Implement authorizations API endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/AuthorizationsEndpoints.cs

### Frontend Implementation for User Story 2

- [ ] T089 [P] [US2] Create authorizations API slice in frontend/src/lib/api/authorizationsApi.ts
- [ ] T090 [P] [US2] Create AuthorizationForm component in frontend/src/components/authorizations/AuthorizationForm.tsx
- [ ] T091 [P] [US2] Create AuthorizationList component in frontend/src/components/authorizations/AuthorizationList.tsx
- [ ] T092 [P] [US2] Create AuthorizationStatusBadge component in frontend/src/components/authorizations/AuthorizationStatusBadge.tsx
- [ ] T093 [US2] Create authorizations page in frontend/src/app/(dashboard)/authorizations/page.tsx
- [ ] T094 [US2] Create new authorization page in frontend/src/app/(dashboard)/authorizations/new/page.tsx
- [ ] T095 [US2] Add authorization actions to client detail page in frontend/src/app/(dashboard)/clients/[id]/page.tsx

**Checkpoint**: User Story 2 complete - tax professionals can create and send 8821 requests

---

## Phase 5: User Story 3 - Client Signs 8821 Authorization (Priority: P3)

**Goal**: Enable clients to sign 8821 forms via secure public link with built-in e-signature

**Independent Test**: Access a signature link as a client, complete the e-signature flow, and verify the signed document is stored

### Tests for User Story 3 (TDD - Write First)

- [ ] T096 [P] [US3] Contract tests for signing endpoints in backend/tests/TranscriptAnalyzer.Contract.Tests/SigningApiTests.cs
- [ ] T097 [P] [US3] Unit tests for SubmitSignatureCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Authorizations/SubmitSignatureCommandTests.cs
- [ ] T098 [P] [US3] Integration tests for signature token validation in backend/tests/TranscriptAnalyzer.Infrastructure.Tests/Security/SignatureTokenServiceTests.cs

### Backend Implementation for User Story 3

- [ ] T099 [P] [US3] Create IESignatureService interface in backend/src/TranscriptAnalyzer.Domain/Interfaces/IESignatureService.cs
- [ ] T100 [P] [US3] Implement BuiltInESignatureService in backend/src/TranscriptAnalyzer.Infrastructure/ESignature/BuiltInESignatureService.cs
- [ ] T101 [P] [US3] Create DocuSignESignatureService (stub for future) in backend/src/TranscriptAnalyzer.Infrastructure/ESignature/DocuSignESignatureService.cs
- [ ] T102 [P] [US3] Create GetSigningAuthorizationQuery in backend/src/TranscriptAnalyzer.Application/Authorizations/Queries/GetSigningAuthorization/
- [ ] T103 [P] [US3] Create SubmitSignatureCommand in backend/src/TranscriptAnalyzer.Application/Authorizations/Commands/SubmitSignature/
- [ ] T104 [US3] Implement signing API endpoints (public, no auth) in backend/src/TranscriptAnalyzer.Api/Endpoints/SigningEndpoints.cs

### Frontend Implementation for User Story 3

- [ ] T105 [P] [US3] Create signing page layout (public) in frontend/src/app/(public)/sign/layout.tsx
- [ ] T106 [P] [US3] Create SignatureForm component in frontend/src/components/signing/SignatureForm.tsx
- [ ] T107 [P] [US3] Create Form8821Preview component in frontend/src/components/signing/Form8821Preview.tsx
- [ ] T108 [US3] Create signing page in frontend/src/app/(public)/sign/[token]/page.tsx
- [ ] T109 [US3] Create signature confirmation page in frontend/src/app/(public)/sign/[token]/complete/page.tsx

**Checkpoint**: User Story 3 complete - clients can sign 8821 via public link

---

## Phase 6: User Story 4 - Authorization Status Dashboard (Priority: P4)

**Goal**: Enable tax professionals to view and filter authorization statuses across all clients

**Independent Test**: Create multiple clients with different authorization statuses and verify the dashboard correctly displays and filters them

### Tests for User Story 4 (TDD - Write First)

- [ ] T110 [P] [US4] Contract tests for dashboard endpoint in backend/tests/TranscriptAnalyzer.Contract.Tests/DashboardApiTests.cs
- [ ] T111 [P] [US4] Unit tests for GetAuthorizationDashboardQuery in backend/tests/TranscriptAnalyzer.Application.Tests/Authorizations/GetAuthorizationDashboardQueryTests.cs

### Backend Implementation for User Story 4

- [ ] T112 [P] [US4] Create AuthorizationDashboardDto in backend/src/TranscriptAnalyzer.Application/Authorizations/AuthorizationDashboardDto.cs
- [ ] T113 [US4] Create GetAuthorizationDashboardQuery in backend/src/TranscriptAnalyzer.Application/Authorizations/Queries/GetAuthorizationDashboard/
- [ ] T114 [US4] Add dashboard endpoint to AuthorizationsEndpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/AuthorizationsEndpoints.cs

### Frontend Implementation for User Story 4

- [ ] T115 [P] [US4] Create DashboardSummaryCard component in frontend/src/components/dashboard/DashboardSummaryCard.tsx
- [ ] T116 [P] [US4] Create AuthorizationStatusChart component in frontend/src/components/dashboard/AuthorizationStatusChart.tsx
- [ ] T117 [P] [US4] Create ExpiringAuthorizationsWidget component in frontend/src/components/dashboard/ExpiringAuthorizationsWidget.tsx
- [ ] T118 [P] [US4] Create RecentActivityFeed component in frontend/src/components/dashboard/RecentActivityFeed.tsx
- [ ] T119 [US4] Create main dashboard page in frontend/src/app/(dashboard)/page.tsx
- [ ] T120 [US4] Add search and filter to authorizations list in frontend/src/components/authorizations/AuthorizationFilters.tsx

**Checkpoint**: User Story 4 complete - dashboard provides operational visibility

---

## Phase 7: User Story 5 - Administrator Manages Organization (Priority: P5)

**Goal**: Enable admins to manage organization settings and team members

**Independent Test**: Create an organization, invite a user, and verify the invited user can log in with appropriate permissions

### Tests for User Story 5 (TDD - Write First)

- [ ] T121 [P] [US5] Contract tests for organizations-api.yaml endpoints in backend/tests/TranscriptAnalyzer.Contract.Tests/OrganizationsApiTests.cs
- [ ] T122 [P] [US5] Unit tests for InviteUserCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Organizations/InviteUserCommandTests.cs
- [ ] T123 [P] [US5] Unit tests for UpdateOrganizationSettingsCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Organizations/UpdateOrganizationSettingsCommandTests.cs

### Backend Implementation for User Story 5

- [ ] T124 [P] [US5] Create OrganizationDto and mappings in backend/src/TranscriptAnalyzer.Application/Organizations/OrganizationDto.cs
- [ ] T125 [P] [US5] Create UserDto and mappings in backend/src/TranscriptAnalyzer.Application/Organizations/UserDto.cs
- [ ] T126 [P] [US5] Create GetCurrentOrganizationQuery in backend/src/TranscriptAnalyzer.Application/Organizations/Queries/GetCurrentOrganization/
- [ ] T127 [P] [US5] Create UpdateOrganizationCommand in backend/src/TranscriptAnalyzer.Application/Organizations/Commands/UpdateOrganization/
- [ ] T128 [P] [US5] Create UpdateOrganizationSettingsCommand in backend/src/TranscriptAnalyzer.Application/Organizations/Commands/UpdateOrganizationSettings/
- [ ] T129 [P] [US5] Create InviteUserCommand in backend/src/TranscriptAnalyzer.Application/Organizations/Commands/InviteUser/
- [ ] T130 [P] [US5] Create UpdateUserCommand in backend/src/TranscriptAnalyzer.Application/Organizations/Commands/UpdateUser/
- [ ] T131 [P] [US5] Create RemoveUserCommand in backend/src/TranscriptAnalyzer.Application/Organizations/Commands/RemoveUser/
- [ ] T132 [P] [US5] Create GetUsersQuery in backend/src/TranscriptAnalyzer.Application/Organizations/Queries/GetUsers/
- [ ] T133 [US5] Implement organizations API endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/OrganizationsEndpoints.cs

### Frontend Implementation for User Story 5

- [ ] T134 [P] [US5] Create organizations API slice in frontend/src/lib/api/organizationsApi.ts
- [ ] T135 [P] [US5] Create OrganizationSettingsForm component in frontend/src/components/settings/OrganizationSettingsForm.tsx
- [ ] T136 [P] [US5] Create TeamMemberList component in frontend/src/components/team/TeamMemberList.tsx
- [ ] T137 [P] [US5] Create InviteUserModal component in frontend/src/components/team/InviteUserModal.tsx
- [ ] T138 [US5] Create settings page in frontend/src/app/(dashboard)/settings/page.tsx
- [ ] T139 [US5] Create team management page in frontend/src/app/(dashboard)/team/page.tsx

**Checkpoint**: User Story 5 complete - admins can manage organization and team

---

## Phase 8: User Story 6 - Secure Transcript Storage (Priority: P6)

**Goal**: Enable secure upload, storage, and retrieval of IRS transcripts

**Independent Test**: Upload a transcript file for a client and verify it is stored, encrypted, and retrievable

### Tests for User Story 6 (TDD - Write First)

- [ ] T140 [P] [US6] Contract tests for transcripts-api.yaml endpoints in backend/tests/TranscriptAnalyzer.Contract.Tests/TranscriptsApiTests.cs
- [ ] T141 [P] [US6] Unit tests for Transcript entity in backend/tests/TranscriptAnalyzer.Domain.Tests/Entities/TranscriptTests.cs
- [ ] T142 [P] [US6] Unit tests for UploadTranscriptCommand in backend/tests/TranscriptAnalyzer.Application.Tests/Transcripts/UploadTranscriptCommandTests.cs
- [ ] T143 [P] [US6] Integration tests for BlobStorageService in backend/tests/TranscriptAnalyzer.Infrastructure.Tests/Storage/BlobStorageServiceTests.cs

### Backend Implementation for User Story 6

- [ ] T144 [US6] Create Transcript entity in backend/src/TranscriptAnalyzer.Domain/Entities/Transcript.cs
- [ ] T145 [US6] Create EF Core configuration for Transcript in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/TranscriptConfiguration.cs
- [ ] T146 [US6] Add Transcript migration in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [ ] T147 [P] [US6] Create TranscriptDto and mappings in backend/src/TranscriptAnalyzer.Application/Transcripts/TranscriptDto.cs
- [ ] T148 [P] [US6] Create UploadTranscriptCommand in backend/src/TranscriptAnalyzer.Application/Transcripts/Commands/UploadTranscript/
- [ ] T149 [P] [US6] Create DeleteTranscriptCommand in backend/src/TranscriptAnalyzer.Application/Transcripts/Commands/DeleteTranscript/
- [ ] T150 [P] [US6] Create GetTranscriptsQuery in backend/src/TranscriptAnalyzer.Application/Transcripts/Queries/GetTranscripts/
- [ ] T151 [P] [US6] Create GetTranscriptDownloadUrlQuery in backend/src/TranscriptAnalyzer.Application/Transcripts/Queries/GetTranscriptDownloadUrl/
- [ ] T152 [P] [US6] Create ValidateTranscriptQuery for file validation in backend/src/TranscriptAnalyzer.Application/Transcripts/Queries/ValidateTranscript/
- [ ] T153 [US6] Implement transcripts API endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/TranscriptsEndpoints.cs

### Frontend Implementation for User Story 6

- [ ] T154 [P] [US6] Create transcripts API slice in frontend/src/lib/api/transcriptsApi.ts
- [ ] T155 [P] [US6] Create TranscriptUploadForm component in frontend/src/components/transcripts/TranscriptUploadForm.tsx
- [ ] T156 [P] [US6] Create TranscriptList component in frontend/src/components/transcripts/TranscriptList.tsx
- [ ] T157 [P] [US6] Create TranscriptViewer component in frontend/src/components/transcripts/TranscriptViewer.tsx
- [ ] T158 [US6] Create transcripts page in frontend/src/app/(dashboard)/transcripts/page.tsx
- [ ] T159 [US6] Create upload transcript page in frontend/src/app/(dashboard)/transcripts/upload/page.tsx
- [ ] T160 [US6] Add transcripts section to client detail page in frontend/src/app/(dashboard)/clients/[id]/page.tsx

**Checkpoint**: User Story 6 complete - transcripts can be uploaded and retrieved securely

---

## Phase 9: Notifications Infrastructure

**Purpose**: Cross-cutting notification system used by multiple stories

- [ ] T161 Create Notification entity in backend/src/TranscriptAnalyzer.Domain/Entities/Notification.cs
- [ ] T162 Create EF Core configuration for Notification in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs
- [ ] T163 Add Notification migration in backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/
- [ ] T164 [P] Create INotificationService interface in backend/src/TranscriptAnalyzer.Domain/Interfaces/INotificationService.cs
- [ ] T165 [P] Implement NotificationService with email + in-app channels in backend/src/TranscriptAnalyzer.Infrastructure/Notifications/NotificationService.cs
- [ ] T166 [P] Configure SignalR hub for real-time notifications in backend/src/TranscriptAnalyzer.Api/Hubs/NotificationHub.cs
- [ ] T167 [P] Create NotificationDto and mappings in backend/src/TranscriptAnalyzer.Application/Notifications/NotificationDto.cs
- [ ] T168 [P] Create GetNotificationsQuery in backend/src/TranscriptAnalyzer.Application/Notifications/Queries/GetNotifications/
- [ ] T169 [P] Create MarkNotificationReadCommand in backend/src/TranscriptAnalyzer.Application/Notifications/Commands/MarkNotificationRead/
- [ ] T170 Implement notifications API endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/NotificationsEndpoints.cs
- [ ] T171 [P] Create notifications API slice in frontend/src/lib/api/notificationsApi.ts
- [ ] T172 [P] Create NotificationBell component with badge in frontend/src/components/notifications/NotificationBell.tsx
- [ ] T173 [P] Create NotificationDropdown component in frontend/src/components/notifications/NotificationDropdown.tsx
- [ ] T174 Add SignalR connection for real-time notifications in frontend/src/lib/notifications/signalr.ts

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T175 [P] Add loading states and skeletons to all list components in frontend/src/components/
- [ ] T176 [P] Add error boundaries and error pages in frontend/src/app/error.tsx
- [ ] T177 [P] Implement optimistic concurrency for Client updates in backend/src/TranscriptAnalyzer.Application/Clients/
- [ ] T178 [P] Add request rate limiting middleware in backend/src/TranscriptAnalyzer.Api/Middleware/RateLimitingMiddleware.cs
- [ ] T179 [P] Configure structured logging with Serilog in backend/src/TranscriptAnalyzer.Api/Configuration/LoggingConfiguration.cs
- [ ] T180 [P] Add health check endpoints in backend/src/TranscriptAnalyzer.Api/Endpoints/HealthEndpoints.cs
- [ ] T181 Create GitHub Actions CI/CD workflow in .github/workflows/ci.yml
- [ ] T182 [P] Add responsive styles for tablet in frontend/src/styles/
- [ ] T183 [P] Add placeholder navigation items for future analysis modules in frontend/src/components/navigation/
- [ ] T184 Run quickstart.md validation - verify local development setup works end-to-end
- [ ] T185 Security audit - verify all PII encryption, no secrets in code, proper RBAC

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-8)**: All depend on Foundational phase completion
  - US1 (Phase 3): No dependencies on other stories - **MVP SCOPE**
  - US2 (Phase 4): Can start after Foundation, uses Client from US1
  - US3 (Phase 5): Depends on US2 (needs Authorization entity)
  - US4 (Phase 6): Can start after Foundation, uses data from US1-US3
  - US5 (Phase 7): Can start after Foundation, independent
  - US6 (Phase 8): Depends on US2 (needs Authorization for validation)
- **Notifications (Phase 9)**: Can start after Foundation, integrates with all stories
- **Polish (Phase 10)**: Depends on all desired user stories being complete

### User Story Dependencies

```
Foundation ──┬── US1 (Clients) ─────── MVP Complete!
             │        │
             │        ├── US2 (Create Auth) ─┬── US3 (Sign Auth)
             │        │                      │
             │        │                      └── US6 (Transcripts)
             │        │
             ├── US4 (Dashboard) ────────────────────────────
             │
             └── US5 (Organization) ─────────────────────────
```

### Parallel Opportunities

Within each phase, tasks marked `[P]` can run in parallel:

**Phase 1**: T002-T006 (all project init), T008-T010 (config)
**Phase 2**: T014-T016 (value objects/enums), T024-T026 (EF configs), T029-T030 (infra services), T045-T048 (foundation tests)
**Phase 3+**: All tests can run in parallel, then models, then services

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (~12 tasks)
2. Complete Phase 2: Foundational (~36 tasks)
3. Complete Phase 3: User Story 1 (~24 tasks)
4. **STOP and VALIDATE**: Tax professionals can create/manage clients
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. **US1** → Client management works → Deploy/Demo (MVP!)
3. **US2** → 8821 creation works → Deploy/Demo
4. **US3** → E-signature works → Deploy/Demo
5. **US4** → Dashboard works → Deploy/Demo
6. **US5** → Organization settings work → Deploy/Demo
7. **US6** → Transcript storage works → Deploy/Demo
8. Phase 9-10 → Polish and notifications

---

## Summary

| Phase | User Story | Task Count | Parallel Tasks |
|-------|-----------|------------|----------------|
| 1 | Setup | 12 | 8 |
| 2 | Foundational | 36 | 22 |
| 3 | US1 - Client Onboarding | 24 | 16 |
| 4 | US2 - Create Authorization | 23 | 16 |
| 5 | US3 - Sign Authorization | 14 | 10 |
| 6 | US4 - Dashboard | 11 | 7 |
| 7 | US5 - Organization | 19 | 14 |
| 8 | US6 - Transcripts | 21 | 14 |
| 9 | Notifications | 14 | 10 |
| 10 | Polish | 11 | 8 |
| **Total** | | **185** | **125** |

**MVP Scope**: Phases 1-3 (72 tasks) → Functional client management with encrypted PII
**Full Feature**: All phases (185 tasks) → Complete MVP foundation

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks in same phase
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- TDD: Write tests first (T049-T052, etc.), ensure they FAIL, then implement
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
