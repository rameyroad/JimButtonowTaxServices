# Implementation Plan: Local Build Support

**Branch**: `002-local-build-support` | **Date**: 2026-02-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-local-build-support/spec.md`

## Summary

Implement full local development environment support using Docker Compose with profile-based service management, following the proven patterns from the agent-manager project. This includes infrastructure services (SQL Server, Redis, Azurite, Seq), application containers (API, Frontend) with hot-reload, optional dev tools (Azure Data Studio, Redis Commander), and lifecycle management scripts.

## Technical Context

**Language/Version**: Bash scripts (POSIX-compatible), Docker Compose V2, Tilt 0.33+
**Primary Dependencies**: Docker Compose, Tilt, .NET 9 SDK image, Node 20 Alpine image
**Storage**: SQL Server 2022 (containerized), Redis 7, Azurite (Azure Storage emulator)
**Testing**: Manual validation via health-check script, service connectivity tests
**Target Platform**: macOS (Darwin), Linux, Windows (via WSL2/Docker Desktop)
**Project Type**: DevOps/Infrastructure tooling for web application
**Performance Goals**: Infrastructure startup < 30s, full stack healthy < 3 minutes
**Constraints**: Must work with existing backend/frontend structure, no source code changes required
**Scale/Scope**: Single developer workstation, supports team of 5-10 developers

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Applies? | Compliance Status |
|-----------|----------|-------------------|
| I. Security-First | Yes | ✅ PASS - Credentials via .env file (not committed), Seq for audit logging |
| II. Test-Driven Development | Limited | ⚠️ N/A - Infrastructure tooling; validation via health checks |
| III. Clean Architecture | No | ✅ N/A - No application code changes |
| IV. API-First Design | No | ✅ N/A - No new API endpoints |
| V. Scalability | Yes | ✅ PASS - Stateless container design, configurable resources |

**Compliance Requirements Check:**
- ✅ Credentials externalized to .env file (never committed)
- ✅ Seq provides audit logging capability for development
- ✅ Local development environment isolated from production data

**Gate Result**: PASS - All applicable principles satisfied.

## Project Structure

### Documentation (this feature)

```text
specs/002-local-build-support/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output (N/A - no data model)
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (N/A - no API contracts)
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
transcript-project/
├── docker/                          # NEW - Docker development environment
│   ├── docker-compose.yml           # Main compose file with profiles
│   ├── tilt-compose.yml             # Tilt-specific compose (no profiles)
│   ├── Tiltfile                     # Tilt configuration
│   ├── .env.template                # Environment template
│   ├── README.md                    # Documentation
│   └── scripts/
│       ├── up.sh                    # Start services
│       ├── down.sh                  # Stop services
│       ├── logs.sh                  # View logs
│       ├── health-check.sh          # Check service health
│       └── tilt-up.sh               # Start with Tilt
├── backend/
│   └── src/                         # Existing .NET source (mounted into container)
├── frontend/
│   └── src/                         # Existing Next.js source (mounted into container)
└── docker-compose.yml               # EXISTING - will be superseded by docker/docker-compose.yml
```

**Structure Decision**: Create new `docker/` directory following agent-manager pattern. This keeps Docker configuration organized and separate from application code. The existing root `docker-compose.yml` will be preserved but the new `docker/` setup will be the primary development environment.

## Complexity Tracking

No constitution violations require justification.
