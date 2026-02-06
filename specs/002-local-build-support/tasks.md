# Tasks: Local Build Support

**Input**: Design documents from `/specs/002-local-build-support/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Manual validation via health-check script - no automated tests required for this infrastructure feature.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Docker config**: `transcript-project/docker/`
- **Scripts**: `transcript-project/docker/scripts/`
- Paths are relative to repository root

---

## Phase 1: Setup (Project Structure)

**Purpose**: Create directory structure and base configuration files

- [x] T001 Create docker directory structure at transcript-project/docker/
- [x] T002 [P] Create .env.template with all environment variables in transcript-project/docker/.env.template
- [x] T003 [P] Add .env to .gitignore in transcript-project/docker/.gitignore
- [x] T004 [P] Create scripts directory at transcript-project/docker/scripts/

---

## Phase 2: Foundational (Core Docker Configuration)

**Purpose**: Core Docker Compose configuration that all profiles depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T005 Define Docker network configuration in transcript-project/docker/docker-compose.yml
- [x] T006 Define named volumes for data persistence in transcript-project/docker/docker-compose.yml
- [x] T007 [P] Configure SQL Server service with health check in transcript-project/docker/docker-compose.yml
- [x] T008 [P] Configure Redis service with health check in transcript-project/docker/docker-compose.yml
- [x] T009 [P] Configure Azurite (Azure Storage emulator) service in transcript-project/docker/docker-compose.yml
- [x] T010 [P] Configure Seq logging service with web UI in transcript-project/docker/docker-compose.yml

**Checkpoint**: Infrastructure services can now be started - foundation ready

---

## Phase 3: User Story 1 - Infrastructure-Only Development (Priority: P1) 🎯 MVP

**Goal**: Developers can start infrastructure services and run API/frontend from IDE

**Independent Test**: Run `./scripts/up.sh`, verify all containers healthy, run API from IDE and confirm database connection

### Implementation for User Story 1

- [x] T011 [US1] Create up.sh script with infrastructure-only default in transcript-project/docker/scripts/up.sh
- [x] T012 [US1] Implement profile selection logic (none=infrastructure) in transcript-project/docker/scripts/up.sh
- [x] T013 [US1] Create health-check.sh script for service status in transcript-project/docker/scripts/health-check.sh
- [x] T014 [US1] Create down.sh script for stopping services in transcript-project/docker/scripts/down.sh
- [x] T015 [US1] Make all scripts executable (chmod +x) in transcript-project/docker/scripts/
- [ ] T016 [US1] Verify infrastructure services start and become healthy within 2 minutes

**Checkpoint**: User Story 1 complete - developers can start infrastructure and run API/frontend from IDE

---

## Phase 4: User Story 2 - Full Stack Docker Development (Priority: P2)

**Goal**: Developers can start all services in containers with hot-reload

**Independent Test**: Run `./scripts/up.sh full`, access web app at localhost:3000, make code change and verify hot-reload

### Implementation for User Story 2

- [x] T017 [P] [US2] Configure API service with dotnet watch in transcript-project/docker/docker-compose.yml
- [x] T018 [P] [US2] Configure Web service with npm dev in transcript-project/docker/docker-compose.yml
- [x] T019 [US2] Add API service dependency on infrastructure health checks in transcript-project/docker/docker-compose.yml
- [x] T020 [US2] Add Web service dependency on API service in transcript-project/docker/docker-compose.yml
- [x] T021 [US2] Configure volume mounts for source code hot-reload in transcript-project/docker/docker-compose.yml
- [x] T022 [US2] Add DOTNET_USE_POLLING_FILE_WATCHER environment variable for API in transcript-project/docker/docker-compose.yml
- [x] T023 [US2] Add WATCHPACK_POLLING environment variable for Web in transcript-project/docker/docker-compose.yml
- [x] T024 [US2] Implement 'full' profile selection in transcript-project/docker/scripts/up.sh
- [ ] T025 [US2] Verify hot-reload works for API code changes
- [ ] T026 [US2] Verify hot-reload works for frontend code changes

**Checkpoint**: User Story 2 complete - full stack runs in Docker with hot-reload

---

## Phase 5: User Story 3 - API-Only Development (Priority: P3)

**Goal**: Frontend developers can run containerized API while debugging frontend in IDE

**Independent Test**: Run `./scripts/up.sh api`, verify API running, start frontend from IDE and confirm connectivity

### Implementation for User Story 3

- [x] T027 [US3] Add 'api' profile to API service definition in transcript-project/docker/docker-compose.yml
- [x] T028 [US3] Implement 'api' profile selection in transcript-project/docker/scripts/up.sh
- [ ] T029 [US3] Verify API profile starts infrastructure + API only
- [ ] T030 [US3] Verify frontend from IDE can connect to containerized API

**Checkpoint**: User Story 3 complete - frontend developers can use containerized backend

---

## Phase 6: User Story 4 - Development Tools Access (Priority: P4)

**Goal**: Developers can access database and cache management tools

**Independent Test**: Run `./scripts/up.sh tools`, access database UI and Redis UI in browser

### Implementation for User Story 4

- [x] T031 [P] [US4] Configure Azure Data Studio web service in transcript-project/docker/docker-compose.yml
- [x] T032 [P] [US4] Configure Redis Commander service in transcript-project/docker/docker-compose.yml
- [x] T033 [US4] Add 'tools' profile to dev tool services in transcript-project/docker/docker-compose.yml
- [x] T034 [US4] Implement 'tools' profile selection in transcript-project/docker/scripts/up.sh
- [ ] T035 [US4] Verify database management UI is accessible
- [ ] T036 [US4] Verify Redis Commander UI is accessible

**Checkpoint**: User Story 4 complete - dev tools accessible for data inspection

---

## Phase 7: User Story 5 - Service Lifecycle Management (Priority: P5)

**Goal**: Developers have helper scripts for all common operations

**Independent Test**: Use each script (up, down, logs, health-check) and verify expected behavior

### Implementation for User Story 5

- [x] T037 [US5] Create logs.sh script with service filtering in transcript-project/docker/scripts/logs.sh
- [x] T038 [US5] Add --follow flag support to logs.sh in transcript-project/docker/scripts/logs.sh
- [x] T039 [US5] Add --wait flag support to health-check.sh in transcript-project/docker/scripts/health-check.sh
- [x] T040 [US5] Add -v flag (remove volumes) to down.sh in transcript-project/docker/scripts/down.sh
- [x] T041 [US5] Add helpful output messages to all scripts
- [ ] T042 [US5] Verify stop command works when services are already stopped
- [ ] T043 [US5] Verify logs command shows service-specific logs

**Checkpoint**: User Story 5 complete - all lifecycle scripts working

---

## Phase 8: Tilt Integration (Enhancement)

**Purpose**: Enhanced hot-reload development with Tilt web UI

- [x] T044 [P] Create tilt-compose.yml without profiles in transcript-project/docker/tilt-compose.yml
- [x] T045 [P] Create Tiltfile with profile configuration in transcript-project/docker/Tiltfile
- [x] T046 Create tilt-up.sh script in transcript-project/docker/scripts/tilt-up.sh
- [ ] T047 Verify Tilt UI accessible at localhost:10350
- [ ] T048 Verify Tilt hot-reload works for API and frontend

---

## Phase 9: Polish & Documentation

**Purpose**: Documentation and final validation

- [x] T049 [P] Create README.md with usage guide in transcript-project/docker/README.md
- [x] T050 [P] Document service URLs and ports in transcript-project/docker/README.md
- [x] T051 [P] Add troubleshooting section to README in transcript-project/docker/README.md
- [ ] T052 Run quickstart.md validation - verify all documented commands work
- [x] T053 Update root README to reference docker/ directory

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-7)**: All depend on Foundational phase completion
  - User stories can then proceed sequentially in priority order (P1 → P2 → P3 → P4 → P5)
  - US2 builds on US1 (profiles extend)
  - US3 builds on US2 (api profile is subset)
  - US4 builds on US2 (tools profile extends full)
  - US5 enhances scripts from US1
- **Tilt Integration (Phase 8)**: Depends on US2 completion (full stack must work)
- **Polish (Phase 9)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - MVP!
- **User Story 2 (P2)**: Builds on US1 - adds API and Web containers
- **User Story 3 (P3)**: Builds on US2 - api profile is a subset
- **User Story 4 (P4)**: Builds on US2 - tools profile extends full
- **User Story 5 (P5)**: Builds on US1 - enhances scripts

### Within Each User Story

- Configuration changes before script updates
- Services must be defined before profile selection
- Verification tasks after implementation

### Parallel Opportunities

**Phase 1 (Setup)**:
```bash
# These can run in parallel:
T002 [P] Create .env.template
T003 [P] Add .gitignore
T004 [P] Create scripts directory
```

**Phase 2 (Foundational)**:
```bash
# These can run in parallel (different services):
T007 [P] Configure SQL Server
T008 [P] Configure Redis
T009 [P] Configure Azurite
T010 [P] Configure Seq
```

**Phase 4 (US2)**:
```bash
# These can run in parallel:
T017 [P] Configure API service
T018 [P] Configure Web service
```

**Phase 6 (US4)**:
```bash
# These can run in parallel:
T031 [P] Configure Azure Data Studio
T032 [P] Configure Redis Commander
```

**Phase 8 (Tilt)**:
```bash
# These can run in parallel:
T044 [P] Create tilt-compose.yml
T045 [P] Create Tiltfile
```

**Phase 9 (Polish)**:
```bash
# These can run in parallel:
T049 [P] Create README.md
T050 [P] Document service URLs
T051 [P] Add troubleshooting
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (infrastructure services)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test `./scripts/up.sh` starts infrastructure
5. Developers can now run API/frontend from IDE with containerized infrastructure

### Incremental Delivery

1. Complete Setup + Foundational → Infrastructure services working
2. Add User Story 1 → Test independently → MVP complete!
3. Add User Story 2 → Full stack in containers
4. Add User Story 3 → API-only profile for frontend devs
5. Add User Story 4 → Dev tools profile
6. Add User Story 5 → Enhanced script features
7. Add Tilt → Enhanced hot-reload experience
8. Add Documentation → Team onboarding ready

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story builds on previous (profiles extend progressively)
- Manual validation via health-check script after each phase
- Reference agent-manager/docker implementation for patterns
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
