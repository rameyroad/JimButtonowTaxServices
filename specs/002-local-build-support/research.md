# Research: Local Build Support

**Feature**: 002-local-build-support
**Date**: 2026-02-05

## Research Summary

This feature adapts the proven Docker development environment pattern from the agent-manager project to the transcript-analyzer project. The reference implementation provides a complete solution that has been validated in production use.

---

## Decision 1: Container Orchestration Approach

**Decision**: Docker Compose V2 with profiles, supplemented by Tilt for enhanced hot-reload

**Rationale**:
- Docker Compose V2 is the standard for local development environments
- Profile support allows flexible service combinations without multiple compose files
- Tilt adds a web UI and superior hot-reload experience for active development
- Both tools are already proven in the agent-manager reference implementation

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| Kubernetes (minikube/kind) | Overkill for local development; complex setup; slower iteration |
| Docker Compose V1 | Deprecated; lacks profile support |
| Podman Compose | Less mature; compatibility issues with some features |
| devcontainers | Requires VS Code; limits IDE choice |

---

## Decision 2: SQL Server Container Strategy

**Decision**: Use mcr.microsoft.com/mssql/server:2022-latest with local volume persistence

**Rationale**:
- Matches production SQL Server version
- Official Microsoft image with regular security updates
- Volume persistence ensures data survives container restarts
- Health check ensures API doesn't start until database is ready

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| PostgreSQL | Project already uses SQL Server; would require migration |
| Azure SQL Edge | Unnecessary for local dev; same SQL Server compatibility |
| SQLite | Insufficient for multi-tenant query testing |

---

## Decision 3: Hot Reload Implementation

**Decision**: `dotnet watch` for API, Watchpack polling for Next.js

**Rationale**:
- `dotnet watch` provides native hot-reload for .NET 9
- `DOTNET_USE_POLLING_FILE_WATCHER=1` required for Docker file system events
- `WATCHPACK_POLLING=true` enables file watching in Node containers
- These settings are proven in the agent-manager implementation

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| Volume sync tools (mutagen) | Additional complexity; polling works reliably |
| Host network mode | Security concerns; port conflict potential |
| Bind mount with inotify | Doesn't work reliably across Docker Desktop for Mac/Windows |

---

## Decision 4: Logging Infrastructure

**Decision**: Seq (datalust/seq) for centralized log aggregation

**Rationale**:
- Structured logging with searchable web UI
- Low resource footprint suitable for local development
- Supports Serilog integration (already in .NET project)
- Free for local development use
- Matches reference implementation pattern

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| ELK Stack | Heavy resource usage; overkill for local dev |
| Loki + Grafana | More complex setup for equivalent functionality |
| Console only | Difficult to search across multiple services |

---

## Decision 5: Profile Organization

**Decision**: Four profiles matching reference pattern

| Profile | Services Included | Use Case |
|---------|-------------------|----------|
| (none/infrastructure) | SQL Server, Redis, Azurite, Seq | Run API/frontend from IDE |
| api | Infrastructure + API | Frontend developers |
| full | All services | Full stack testing |
| tools | Full + Azure Data Studio + Redis Commander | Data inspection |

**Rationale**:
- Matches developer workflow patterns
- Infrastructure-only as default for IDE debugging
- Progressive inclusion for different needs

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| Separate compose files | Harder to maintain; profile approach cleaner |
| All services always | Wastes resources when not needed |
| More granular profiles | Diminishing returns; four profiles cover all cases |

---

## Decision 6: Script Implementation

**Decision**: Bash scripts with POSIX compatibility

**Rationale**:
- Works on macOS, Linux, and Windows (Git Bash/WSL2)
- Simple to understand and modify
- No additional runtime dependencies
- Matches reference implementation

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| PowerShell | Not available on all platforms without extra install |
| Make | Steeper learning curve; overkill for simple commands |
| Node.js scripts | Requires Node runtime on host |
| Python scripts | Requires Python runtime on host |

---

## Decision 7: Environment Configuration

**Decision**: `.env.template` with all configurable values, `.env` gitignored

**Rationale**:
- Standard Docker Compose pattern
- Secrets never committed to repository
- Easy customization of ports and credentials
- Template documents all available options

**Environment Variables**:
| Variable | Default | Purpose |
|----------|---------|---------|
| API_PORT | 5000 | .NET API port |
| WEB_PORT | 3000 | Next.js frontend port |
| SQL_PORT | 1433 | SQL Server port |
| REDIS_PORT | 6379 | Redis port |
| SEQ_UI_PORT | 8081 | Seq web UI port |
| SEQ_INGESTION_PORT | 5341 | Seq log ingestion |
| AZURITE_BLOB_PORT | 10000 | Azure Blob emulator |
| USE_DEV_AUTH | true | Bypass Auth0 for local dev |

---

## Decision 8: Dev Tools Selection

**Decision**: Azure Data Studio (via web) and Redis Commander for optional tools profile

**Rationale**:
- Azure Data Studio provides SQL Server management without local install
- Redis Commander provides Redis inspection without CLI expertise
- Both are lightweight containerized tools
- Optional via profile - don't consume resources unless needed

**Alternatives Considered**:
| Alternative | Rejected Because |
|-------------|------------------|
| pgAdmin | Wrong database type (PostgreSQL) |
| DBeaver | Heavier; requires separate install |
| RedisInsight | Heavier than Redis Commander |

---

## Technical Dependencies

### Required on Host Machine
- Docker Desktop 4.x (or Docker Engine + Compose V2)
- Git (for scripts)
- 8GB+ RAM recommended
- ~5GB disk space for images

### Optional on Host Machine
- Tilt 0.33+ (for enhanced hot-reload)
- .NET 9 SDK (if running API from IDE)
- Node.js 20+ (if running frontend from IDE)

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Port conflicts | Medium | Low | Configurable via .env |
| SQL Server ARM compatibility | Low | Medium | Works on Apple Silicon via emulation |
| File watching performance | Medium | Low | Polling fallback enabled |
| Docker resource limits | Low | Medium | Documentation on recommended settings |

---

## References

- Agent Manager Docker Implementation: `/Users/jason/src/rameyroad/agent-manager/docker/`
- Docker Compose Profiles: https://docs.docker.com/compose/profiles/
- Tilt Documentation: https://docs.tilt.dev/
- Seq Documentation: https://docs.datalust.co/docs
