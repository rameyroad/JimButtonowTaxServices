# Tasks: Client Management

**Input**: Design documents from `/specs/004-client-management/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/clients-api.yaml

**Tests**: Included per Constitution II (Test-Driven Development) - 80% coverage required

**Organization**: Tasks grouped by user story for independent implementation and testing

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/TranscriptAnalyzer.{Domain,Application,Api,Infrastructure}/`
- **Frontend**: `frontend/src/{app,components,lib}/`
- **Tests**: `backend/tests/`, `frontend/tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Domain model extensions and foundational changes required by all user stories

- [ ] T001 Extend BusinessEntityType enum with NonProfit, Trust, Estate in `backend/src/TranscriptAnalyzer.Domain/Enums/BusinessEntityType.cs`
- [ ] T002 Add Archive() and Restore() methods to Client entity in `backend/src/TranscriptAnalyzer.Domain/Entities/Client.cs`
- [ ] T003 Add version field to Client entity for optimistic concurrency in `backend/src/TranscriptAnalyzer.Domain/Entities/Client.cs`
- [ ] T004 [P] Create EF Core migration for BusinessEntityType and version field in `backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Migrations/`
- [ ] T005 [P] Update ClientConfiguration for version field in `backend/src/TranscriptAnalyzer.Infrastructure/Persistence/Configurations/ClientConfiguration.cs`
- [ ] T006 Apply database migration and verify schema changes

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: DTOs, base infrastructure, and API structure that ALL user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T007 [P] Create ClientDto record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/ClientDto.cs`
- [ ] T008 [P] Create ClientListItemDto record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/ClientListItemDto.cs`
- [ ] T009 [P] Create ClientDetailDto record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/ClientDetailDto.cs`
- [ ] T010 [P] Create AddressDto record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/AddressDto.cs`
- [ ] T011 [P] Create CreateClientRequest record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/CreateClientRequest.cs`
- [ ] T012 [P] Create UpdateClientRequest record in `backend/src/TranscriptAnalyzer.Application/Clients/DTOs/UpdateClientRequest.cs`
- [ ] T013 Create AutoMapper profile for Client DTOs in `backend/src/TranscriptAnalyzer.Application/Clients/ClientMappingProfile.cs`
- [ ] T014 Create ClientsEndpoints stub with route group in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T015 Register ClientsEndpoints in Program.cs in `backend/src/TranscriptAnalyzer.Api/Program.cs`
- [ ] T016 [P] Create RTK Query API slice for clients in `frontend/src/lib/api/clientsApi.ts`
- [ ] T017 [P] Create TypeScript types matching API contract in `frontend/src/lib/types/client.ts`
- [ ] T018 Register clients API slice in Redux store in `frontend/src/lib/store.ts`

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - View Client List (Priority: P1) 🎯 MVP

**Goal**: Tax professionals can see all clients in a searchable, sortable, paginated list

**Independent Test**: Navigate to /clients, verify list displays with pagination, search, and sort working

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T019 [P] [US1] Contract test for GET /clients in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ListClientsTests.cs`
- [ ] T020 [P] [US1] Contract test for pagination and sorting in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ListClientsTests.cs`
- [ ] T021 [P] [US1] Contract test for search functionality in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ListClientsTests.cs`
- [ ] T022 [P] [US1] Contract test for role-based list access (all roles can view) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ListClientsTests.cs`
- [ ] T023 [P] [US1] Contract test for tenant isolation (RLS) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ListClientsTests.cs`

### Implementation for User Story 1

- [ ] T024 [P] [US1] Create ListClientsQuery record in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/ListClients/ListClientsQuery.cs`
- [ ] T025 [US1] Create ListClientsHandler with pagination, search, sort in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/ListClients/ListClientsHandler.cs`
- [ ] T026 [US1] Implement GET /clients endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T027 [US1] Add authorization policy (all authenticated users) for list endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T028 [P] [US1] Create ClientList component with MUI DataGrid in `frontend/src/components/clients/ClientList.tsx`
- [ ] T029 [P] [US1] Create ClientTypeBadge component in `frontend/src/components/clients/ClientTypeBadge.tsx`
- [ ] T030 [P] [US1] Create TaxIdentifierDisplay component for masked display in `frontend/src/components/clients/TaxIdentifierDisplay.tsx`
- [ ] T031 [US1] Create clients list page at `frontend/src/app/(dashboard)/clients/page.tsx`
- [ ] T032 [US1] Add clients navigation item to sidebar in `frontend/src/app/(dashboard)/layout.tsx`
- [ ] T033 [US1] Verify all US1 contract tests pass

**Checkpoint**: User Story 1 complete - client list is functional and testable

---

## Phase 4: User Story 2 - Create Individual Client (Priority: P2)

**Goal**: Tax professionals can add new individual clients with SSN

**Independent Test**: Click "Add Client", select Individual, fill form, save, verify client in list with SSN masked

### Tests for User Story 2

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T034 [P] [US2] Contract test for POST /clients (individual) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T035 [P] [US2] Contract test for SSN validation (format XXX-XX-XXXX) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T036 [P] [US2] Contract test for required fields validation (individual) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T037 [P] [US2] Contract test for duplicate SSN detection (409 Conflict) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T038 [P] [US2] Contract test for role restriction (ReadOnly denied, TaxProfessional/Admin allowed) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T039 [P] [US2] Unit test for SSN encryption in `backend/tests/TranscriptAnalyzer.Application.Tests/Clients/CreateClientTests.cs`

### Implementation for User Story 2

- [ ] T040 [P] [US2] Create CreateClientCommand record in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientCommand.cs`
- [ ] T041 [P] [US2] Create CreateClientValidator with FluentValidation in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientValidator.cs`
- [ ] T042 [US2] Create CreateClientHandler with encryption in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientHandler.cs`
- [ ] T043 [US2] Add duplicate SSN detection logic in CreateClientHandler in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientHandler.cs`
- [ ] T044 [US2] Implement POST /clients endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T045 [US2] Add write:clients authorization policy for create endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T046 [P] [US2] Create TaxIdentifierInput component with masking in `frontend/src/components/clients/TaxIdentifierInput.tsx`
- [ ] T047 [P] [US2] Create AddressForm component in `frontend/src/components/clients/AddressForm.tsx`
- [ ] T048 [US2] Create ClientForm component for individual clients in `frontend/src/components/clients/ClientForm.tsx`
- [ ] T049 [US2] Create clients/new page at `frontend/src/app/(dashboard)/clients/new/page.tsx`
- [ ] T050 [US2] Add "Add Client" button to list (hide for ReadOnly) in `frontend/src/components/clients/ClientList.tsx`
- [ ] T051 [US2] Handle duplicate SSN warning dialog in `frontend/src/components/clients/ClientForm.tsx`
- [ ] T052 [US2] Verify all US2 contract tests pass

**Checkpoint**: User Story 2 complete - individual client creation works end-to-end

---

## Phase 5: User Story 3 - Create Business Client (Priority: P2)

**Goal**: Tax professionals can add new business clients with EIN

**Independent Test**: Click "Add Client", select Business, fill form with entity type, save, verify client in list

### Tests for User Story 3

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T053 [P] [US3] Contract test for POST /clients (business) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T054 [P] [US3] Contract test for EIN validation (format XX-XXXXXXX) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T055 [P] [US3] Contract test for required fields validation (business) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`
- [ ] T056 [P] [US3] Contract test for all entity types (including NonProfit, Trust, Estate) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/CreateClientTests.cs`

### Implementation for User Story 3

- [ ] T057 [US3] Extend CreateClientValidator for business fields in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientValidator.cs`
- [ ] T058 [US3] Extend CreateClientHandler for business type in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/CreateClient/CreateClientHandler.cs`
- [ ] T059 [US3] Extend ClientForm for business type with entity type dropdown in `frontend/src/components/clients/ClientForm.tsx`
- [ ] T060 [P] [US3] Create EntityTypeSelect component in `frontend/src/components/clients/EntityTypeSelect.tsx`
- [ ] T061 [US3] Add client type selection (Individual/Business) toggle to ClientForm in `frontend/src/components/clients/ClientForm.tsx`
- [ ] T062 [US3] Verify all US3 contract tests pass

**Checkpoint**: User Story 3 complete - business client creation works end-to-end

---

## Phase 6: User Story 4 - View and Edit Client Details (Priority: P3)

**Goal**: Tax professionals can view complete client info and update contact details/address

**Independent Test**: Click client row, view details with masked PII, edit contact info, save, verify changes persist

### Tests for User Story 4

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T063 [P] [US4] Contract test for GET /clients/{id} in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/GetClientTests.cs`
- [ ] T064 [P] [US4] Contract test for PATCH /clients/{id} in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/UpdateClientTests.cs`
- [ ] T065 [P] [US4] Contract test for optimistic concurrency (version conflict) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/UpdateClientTests.cs`
- [ ] T066 [P] [US4] Contract test for role restriction on edit (ReadOnly denied) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/UpdateClientTests.cs`
- [ ] T067 [P] [US4] Contract test for audit logging on update in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/UpdateClientTests.cs`
- [ ] T068 [P] [US4] Contract test verifying full SSN/EIN never returned in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/GetClientTests.cs`

### Implementation for User Story 4

- [ ] T069 [P] [US4] Create GetClientQuery record in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/GetClient/GetClientQuery.cs`
- [ ] T070 [US4] Create GetClientHandler in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/GetClient/GetClientHandler.cs`
- [ ] T071 [P] [US4] Create UpdateClientCommand record in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/UpdateClient/UpdateClientCommand.cs`
- [ ] T072 [P] [US4] Create UpdateClientValidator in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/UpdateClient/UpdateClientValidator.cs`
- [ ] T073 [US4] Create UpdateClientHandler with optimistic concurrency in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/UpdateClient/UpdateClientHandler.cs`
- [ ] T074 [US4] Add audit logging to UpdateClientHandler in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/UpdateClient/UpdateClientHandler.cs`
- [ ] T075 [US4] Implement GET /clients/{id} endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T076 [US4] Implement PATCH /clients/{id} endpoint in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T077 [P] [US4] Create ClientDetail component in `frontend/src/components/clients/ClientDetail.tsx`
- [ ] T078 [US4] Create client detail page at `frontend/src/app/(dashboard)/clients/[id]/page.tsx`
- [ ] T079 [US4] Create client edit page at `frontend/src/app/(dashboard)/clients/[id]/edit/page.tsx`
- [ ] T080 [US4] Add Edit button to detail page (hide for ReadOnly) in `frontend/src/components/clients/ClientDetail.tsx`
- [ ] T081 [US4] Handle concurrent edit conflict in edit page in `frontend/src/app/(dashboard)/clients/[id]/edit/page.tsx`
- [ ] T082 [US4] Verify all US4 contract tests pass

**Checkpoint**: User Story 4 complete - view and edit functionality works end-to-end

---

## Phase 7: User Story 5 - Role-Based Access Control (Priority: P3)

**Goal**: Organization admins can control what actions team members perform on client data

**Independent Test**: Create users with different roles, verify each can only perform authorized actions

### Tests for User Story 5

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T083 [P] [US5] Contract test for Admin full access in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/RbacTests.cs`
- [ ] T084 [P] [US5] Contract test for TaxProfessional view/create/edit (no delete) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/RbacTests.cs`
- [ ] T085 [P] [US5] Contract test for ReadOnly view only (no create/edit/delete) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/RbacTests.cs`
- [ ] T086 [P] [US5] Contract test for tenant isolation across organizations in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/RbacTests.cs`
- [ ] T087 [P] [US5] Integration test for RLS enforcement at database level in `backend/tests/TranscriptAnalyzer.Infrastructure.Tests/Persistence/RlsClientTests.cs`

### Implementation for User Story 5

- [ ] T088 [US5] Create role-based authorization policy extensions in `backend/src/TranscriptAnalyzer.Api/Configuration/ClientAuthorizationPolicies.cs`
- [ ] T089 [US5] Apply authorization policies to all client endpoints in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T090 [P] [US5] Create useClientPermissions hook in `frontend/src/lib/hooks/useClientPermissions.ts`
- [ ] T091 [US5] Apply role-based UI visibility to ClientList in `frontend/src/components/clients/ClientList.tsx`
- [ ] T092 [US5] Apply role-based UI visibility to ClientDetail in `frontend/src/components/clients/ClientDetail.tsx`
- [ ] T093 [US5] Apply role-based UI visibility to ClientForm in `frontend/src/components/clients/ClientForm.tsx`
- [ ] T094 [US5] Verify all US5 contract tests pass

**Checkpoint**: User Story 5 complete - RBAC enforced on frontend and backend

---

## Phase 8: User Story 6 - Delete/Archive Client (Priority: P4)

**Goal**: Administrators can archive clients while maintaining audit history

**Independent Test**: As Admin, archive client, verify hidden from list, show in archived view, restore client

### Tests for User Story 6

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T095 [P] [US6] Contract test for DELETE /clients/{id} (archive) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ArchiveClientTests.cs`
- [ ] T096 [P] [US6] Contract test for POST /clients/{id}/restore in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ArchiveClientTests.cs`
- [ ] T097 [P] [US6] Contract test for archive role restriction (Admin only) in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ArchiveClientTests.cs`
- [ ] T098 [P] [US6] Contract test for includeArchived query parameter in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ArchiveClientTests.cs`
- [ ] T099 [P] [US6] Contract test for archived client not in default list in `backend/tests/TranscriptAnalyzer.Api.Tests/Clients/ArchiveClientTests.cs`

### Implementation for User Story 6

- [ ] T100 [P] [US6] Create ArchiveClientCommand record in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/ArchiveClient/ArchiveClientCommand.cs`
- [ ] T101 [US6] Create ArchiveClientHandler in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/ArchiveClient/ArchiveClientHandler.cs`
- [ ] T102 [P] [US6] Create RestoreClientCommand record in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/RestoreClient/RestoreClientCommand.cs`
- [ ] T103 [US6] Create RestoreClientHandler in `backend/src/TranscriptAnalyzer.Application/Clients/Commands/RestoreClient/RestoreClientHandler.cs`
- [ ] T104 [US6] Implement DELETE /clients/{id} endpoint (Admin only) in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T105 [US6] Implement POST /clients/{id}/restore endpoint (Admin only) in `backend/src/TranscriptAnalyzer.Api/Endpoints/ClientsEndpoints.cs`
- [ ] T106 [US6] Add includeArchived parameter to ListClientsQuery in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/ListClients/ListClientsQuery.cs`
- [ ] T107 [US6] Update ListClientsHandler to filter archived by default in `backend/src/TranscriptAnalyzer.Application/Clients/Queries/ListClients/ListClientsHandler.cs`
- [ ] T108 [P] [US6] Create ArchiveConfirmDialog component in `frontend/src/components/clients/ArchiveConfirmDialog.tsx`
- [ ] T109 [US6] Add Archive button to ClientDetail (Admin only) in `frontend/src/components/clients/ClientDetail.tsx`
- [ ] T110 [US6] Add "Show Archived" toggle to ClientList (Admin only) in `frontend/src/components/clients/ClientList.tsx`
- [ ] T111 [US6] Add visual distinction for archived clients in list in `frontend/src/components/clients/ClientList.tsx`
- [ ] T112 [US6] Add Restore button for archived clients (Admin only) in `frontend/src/components/clients/ClientDetail.tsx`
- [ ] T113 [US6] Verify all US6 contract tests pass

**Checkpoint**: User Story 6 complete - archive/restore functionality works end-to-end

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: E2E testing, performance, and final quality checks

- [ ] T114 [P] Add Playwright E2E test for client list navigation in `frontend/tests/e2e/clients/list.spec.ts`
- [ ] T115 [P] Add Playwright E2E test for create individual client flow in `frontend/tests/e2e/clients/create-individual.spec.ts`
- [ ] T116 [P] Add Playwright E2E test for create business client flow in `frontend/tests/e2e/clients/create-business.spec.ts`
- [ ] T117 [P] Add Playwright E2E test for edit client flow in `frontend/tests/e2e/clients/edit.spec.ts`
- [ ] T118 [P] Add Playwright E2E test for archive/restore flow in `frontend/tests/e2e/clients/archive.spec.ts`
- [ ] T119 Performance test: verify list loads < 2 seconds with 1,000 clients
- [ ] T120 Security review: verify no PII leakage in logs or responses
- [ ] T121 Run full test suite and verify 80% backend coverage
- [ ] T122 Update quickstart.md with client management testing instructions in `specs/004-client-management/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 - BLOCKS all user stories
- **User Stories (Phase 3-8)**: All depend on Phase 2 completion
  - Can proceed in priority order (P1 → P2 → P3 → P4)
  - Or in parallel if multiple developers
- **Polish (Phase 9)**: Depends on all user stories being complete

### User Story Dependencies

| Story | Priority | Depends On | Can Start After |
|-------|----------|------------|-----------------|
| US1 - View Client List | P1 | Phase 2 | Foundational complete |
| US2 - Create Individual | P2 | US1 (list to verify) | US1 complete |
| US3 - Create Business | P2 | US2 (shares form) | US2 complete |
| US4 - View/Edit Details | P3 | US1 (navigation) | US1 complete |
| US5 - RBAC | P3 | US1-US4 (applies to all) | US4 complete |
| US6 - Archive/Delete | P4 | US1, US5 (RBAC) | US5 complete |

### Within Each User Story

1. Contract tests MUST be written and FAIL before implementation
2. Backend handlers before API endpoints
3. API endpoints before frontend components
4. Frontend components before pages
5. All story tests must pass before moving to next story

### Parallel Opportunities

```text
Phase 1 (Setup):
- T004 [P] and T005 [P] can run together

Phase 2 (Foundational):
- T007-T012 [P] all DTOs can be created in parallel
- T016-T017 [P] frontend types can be created in parallel

Phase 3 (US1):
- T019-T023 [P] all contract tests in parallel
- T028-T030 [P] all components in parallel

Phase 4 (US2):
- T034-T039 [P] all contract tests in parallel
- T046-T047 [P] components in parallel

Phase 5 (US3):
- T053-T056 [P] all contract tests in parallel

Phase 6 (US4):
- T063-T068 [P] all contract tests in parallel
- T069, T071-T072 [P] query/command records in parallel

Phase 7 (US5):
- T083-T087 [P] all contract tests in parallel

Phase 8 (US6):
- T095-T099 [P] all contract tests in parallel
- T100, T102 [P] command records in parallel

Phase 9 (Polish):
- T114-T118 [P] all E2E tests in parallel
```

---

## Parallel Example: User Story 1

```bash
# Step 1: Write all contract tests in parallel (ensure they FAIL):
Task T019: Contract test for GET /clients
Task T020: Contract test for pagination and sorting
Task T021: Contract test for search functionality
Task T022: Contract test for role-based list access
Task T023: Contract test for tenant isolation

# Step 2: Implement backend query (sequential):
Task T024: Create ListClientsQuery
Task T025: Create ListClientsHandler
Task T026: Implement GET endpoint
Task T027: Add authorization policy

# Step 3: Create frontend components in parallel:
Task T028: ClientList component
Task T029: ClientTypeBadge component
Task T030: TaxIdentifierDisplay component

# Step 4: Create page and integration (sequential):
Task T031: Create clients list page
Task T032: Add navigation item
Task T033: Verify all tests pass
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T006)
2. Complete Phase 2: Foundational (T007-T018) - **CRITICAL**
3. Complete Phase 3: User Story 1 (T019-T033)
4. **STOP and VALIDATE**: Test client list independently
5. Deploy/demo if ready - users can view existing clients

### Incremental Delivery

1. **MVP**: Setup + Foundational + US1 → View client list
2. **+US2**: Create individual clients → Onboard taxpayers
3. **+US3**: Create business clients → Full client types
4. **+US4**: View/edit details → Maintain client data
5. **+US5**: RBAC → Secure multi-user access
6. **+US6**: Archive/restore → Data lifecycle management

### Sequential Team Strategy

Single developer priority order:
1. Phase 1 → Phase 2 → Phase 3 (MVP)
2. Phase 4 → Phase 5 (extends client creation)
3. Phase 6 → Phase 7 (view/edit and security)
4. Phase 8 → Phase 9 (archive and polish)

### Parallel Team Strategy

With 2+ developers after Phase 2 completes:
- Developer A: US1 → US4 → polish
- Developer B: US2 → US3 → US5 → US6

---

## Summary

| Phase | Story | Task Count | Parallel Tasks |
|-------|-------|------------|----------------|
| 1 | Setup | 6 | 2 |
| 2 | Foundational | 12 | 8 |
| 3 | US1 - View List | 15 | 8 |
| 4 | US2 - Create Individual | 19 | 10 |
| 5 | US3 - Create Business | 10 | 4 |
| 6 | US4 - View/Edit | 20 | 10 |
| 7 | US5 - RBAC | 12 | 6 |
| 8 | US6 - Archive | 19 | 7 |
| 9 | Polish | 9 | 5 |
| **Total** | | **122** | **60 (49%)** |

---

## Notes

- [P] tasks = different files, no dependencies between them
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing (TDD per Constitution)
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Constitution requires 80% backend test coverage
