# Feature Specification: Local Build Support

**Feature Branch**: `002-local-build-support`
**Created**: 2026-02-05
**Status**: Draft
**Input**: User description: "I need to have full local build support following the patterns in @/Users/jason/src/rameyroad/agent-manager/docker"

## Clarifications

### Session 2026-02-05

- Q: Should this local build support include a centralized logging service? → A: Yes, include Seq for centralized log aggregation with web UI (matches reference pattern)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Infrastructure-Only Development (Priority: P1)

As a developer, I want to start only the infrastructure services (database, cache, storage emulator) so I can run the API and frontend from my IDE with debugging capabilities.

**Why this priority**: This is the most common development workflow - developers need infrastructure running while they debug code locally in their IDE with full debugging support, breakpoints, and hot reload.

**Independent Test**: Can be fully tested by running a single command to start infrastructure, then running the API and frontend from the IDE - developer can make code changes and see them reflected immediately.

**Acceptance Scenarios**:

1. **Given** Docker is installed and running, **When** developer runs the start infrastructure command, **Then** SQL Server, Redis, Azurite, and Seq containers start and become healthy within 2 minutes
2. **Given** infrastructure services are running, **When** developer runs API from IDE, **Then** API connects to containerized database, Redis, and Azurite successfully
3. **Given** infrastructure services are running, **When** developer runs frontend from IDE, **Then** frontend connects to API and functions correctly

---

### User Story 2 - Full Stack Docker Development (Priority: P2)

As a developer, I want to start all services (infrastructure + API + frontend) in Docker containers so I can test the full application stack without IDE setup.

**Why this priority**: Enables testing the complete application in a containerized environment similar to production, useful for integration testing and team members who don't need to debug code.

**Independent Test**: Can be fully tested by running a single command that starts all services, then accessing the web application in a browser and performing basic operations.

**Acceptance Scenarios**:

1. **Given** Docker is installed, **When** developer runs the full stack command, **Then** all services start with proper dependency ordering (infrastructure first, then API, then frontend)
2. **Given** full stack is running, **When** developer accesses the web application, **Then** the application loads and displays correctly
3. **Given** full stack is running, **When** developer makes a code change in the API source, **Then** the change is reflected via hot reload without manual restart

---

### User Story 3 - API-Only Development (Priority: P3)

As a frontend developer, I want to start infrastructure and API in Docker while running the frontend from my IDE so I can focus on frontend development with debugging.

**Why this priority**: Common workflow for frontend developers who need a running backend but want to debug frontend code locally.

**Independent Test**: Can be fully tested by starting API profile, then running frontend from IDE and making UI changes that reflect immediately.

**Acceptance Scenarios**:

1. **Given** Docker is running, **When** developer runs the API profile command, **Then** infrastructure and API start but not the frontend container
2. **Given** API profile is running, **When** developer starts frontend from IDE, **Then** frontend connects to containerized API successfully

---

### User Story 4 - Development Tools Access (Priority: P4)

As a developer, I want optional access to database and cache management tools so I can inspect data and troubleshoot issues.

**Why this priority**: Useful for debugging and data inspection, but not required for primary development workflows.

**Independent Test**: Can be tested by starting the tools profile and accessing management UIs to view database tables and cache entries.

**Acceptance Scenarios**:

1. **Given** developer requests tools profile, **When** services start, **Then** database management UI and Redis management UI are accessible
2. **Given** tools are running, **When** developer accesses database management UI, **Then** developer can view and query database tables

---

### User Story 5 - Service Lifecycle Management (Priority: P5)

As a developer, I want helper scripts to manage service lifecycle so I can easily start, stop, view logs, and check health status.

**Why this priority**: Quality-of-life improvement that makes the development experience smoother.

**Independent Test**: Can be tested by using each script and verifying expected behavior.

**Acceptance Scenarios**:

1. **Given** services are running, **When** developer runs stop command, **Then** all services stop gracefully
2. **Given** services are running, **When** developer runs logs command for a specific service, **Then** logs for that service are displayed
3. **Given** services are starting, **When** developer runs health check command, **Then** status of all services is displayed

---

### Edge Cases

- What happens when required ports are already in use by other applications?
- How does system handle Docker not being installed or running?
- What happens when a service fails health check during startup?
- How does system handle database connection failures?
- What happens when developer runs stop command while services are already stopped?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a Docker Compose configuration that defines all required services (SQL Server, Redis, Azurite, Seq, .NET API, Next.js frontend)
- **FR-002**: System MUST support profile-based startup allowing developers to start subsets of services (infrastructure-only, API-only, full-stack, tools)
- **FR-003**: System MUST configure proper service dependency ordering so infrastructure starts before applications
- **FR-004**: System MUST provide health checks for all critical services to ensure reliable startup sequencing
- **FR-005**: System MUST support hot reload for both API and frontend code when running in containers
- **FR-006**: System MUST provide volume mounting to persist data across container restarts
- **FR-007**: System MUST provide environment variable configuration via .env file for customizable ports and settings
- **FR-008**: System MUST provide shell scripts for common operations: start, stop, logs, health-check
- **FR-009**: System MUST support Tilt integration for enhanced hot-reload development with web UI
- **FR-010**: System MUST provide sensible default configuration that works out-of-the-box with minimal setup
- **FR-011**: System MUST isolate containers on a dedicated Docker network
- **FR-012**: System MUST provide documentation explaining usage, profiles, and troubleshooting
- **FR-013**: System MUST include Seq for centralized log aggregation with a searchable web UI accessible to developers

### Key Entities

- **Service**: A containerized application component (SQL Server, Redis, Azurite, Seq, API, Frontend, Dev Tools)
- **Profile**: A named collection of services to start together (infrastructure, api, full, tools)
- **Volume**: Persistent storage for service data that survives container restarts
- **Environment Configuration**: Settings that customize service behavior (ports, credentials, feature flags)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Developer can start infrastructure services with a single command in under 30 seconds (excluding image download time)
- **SC-002**: Developer can start full stack with a single command and have all services healthy within 3 minutes
- **SC-003**: Code changes in API are reflected within 5 seconds when running with hot reload
- **SC-004**: Code changes in frontend are reflected within 3 seconds when running with hot reload
- **SC-005**: Developer can customize all port mappings via environment configuration to avoid conflicts
- **SC-006**: Services remain running and healthy during normal development for at least 8 hours without manual intervention
- **SC-007**: Developer can completely tear down all services and volumes with a single command
- **SC-008**: New team members can get the local environment running within 15 minutes using documentation

## Assumptions

- Docker Desktop (or Docker Engine with Docker Compose V2) is installed on the developer's machine
- Developers have at least 8GB of RAM available for running all services
- The development machine has sufficient disk space for Docker images and volumes (approximately 5GB)
- Developers are familiar with basic Docker and command-line operations
- The project uses .NET 9 for the backend API and Next.js 14+ for the frontend
- SQL Server is the primary database (matching the existing project configuration)
- Auth0 is used for authentication with a development bypass mode available
