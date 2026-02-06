# Data Model: Local Build Support

**Feature**: 002-local-build-support
**Date**: 2026-02-05

## Overview

This feature is infrastructure tooling and does not introduce new application data models. The "entities" below describe the Docker Compose configuration structure rather than database entities.

## Configuration Entities

### Service Definition

Represents a containerized service in the Docker Compose configuration.

| Field | Type | Description |
|-------|------|-------------|
| name | string | Service identifier (e.g., "sqlserver", "api", "web") |
| image | string | Docker image reference |
| profiles | string[] | Which profiles include this service |
| ports | string[] | Port mappings (host:container) |
| environment | map | Environment variables |
| volumes | string[] | Volume mounts |
| depends_on | map | Service dependencies with conditions |
| healthcheck | object | Health check configuration |
| networks | string[] | Network attachments |

### Profile Definition

Represents a named collection of services.

| Profile | Services |
|---------|----------|
| (default) | sqlserver, redis, azurite, seq |
| api | (default) + api |
| full | api + web |
| tools | full + azuredatastudio, redis-commander |

### Volume Definition

Represents persistent storage.

| Volume | Purpose | Persistence |
|--------|---------|-------------|
| sqlserver-data | SQL Server database files | Survives restart |
| redis-data | Redis persistence | Survives restart |
| azurite-data | Azure Storage emulator data | Survives restart |
| seq-data | Seq log storage | Survives restart |
| api-nuget-cache | NuGet package cache | Survives restart |
| web-node-modules | Node modules cache | Survives restart |

### Environment Configuration

Configuration loaded from `.env` file.

| Category | Variables |
|----------|-----------|
| Ports | API_PORT, WEB_PORT, SQL_PORT, REDIS_PORT, SEQ_UI_PORT, SEQ_INGESTION_PORT, AZURITE_*_PORT |
| Credentials | SA_PASSWORD |
| Feature Flags | USE_DEV_AUTH |
| Connection Strings | (Derived from other variables) |

## State Transitions

### Service Lifecycle

```
[Not Running] → [Starting] → [Healthy] → [Running]
                    ↓            ↓
              [Unhealthy]   [Stopping]
                    ↓            ↓
              [Restarting]  [Stopped]
```

### Profile Startup Sequence

```
1. Infrastructure services start in parallel:
   - sqlserver (healthcheck: SQL query)
   - redis (healthcheck: PING)
   - azurite (no healthcheck, fast start)
   - seq (no healthcheck, fast start)

2. Wait for infrastructure health checks

3. If profile includes 'api':
   - api starts (depends_on: sqlserver healthy, redis healthy, seq started)
   - api healthcheck: HTTP /health

4. If profile includes 'web':
   - web starts (depends_on: api started)
   - web healthcheck: HTTP /

5. If profile includes 'tools':
   - Dev tools start (depends_on: respective services)
```

## Relationships

```
┌─────────────────────────────────────────────────────────────┐
│                      Docker Network                          │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────┐   ┌───────┐   ┌─────────┐   ┌─────┐          │
│  │ sqlserver│   │ redis │   │ azurite │   │ seq │          │
│  └────┬─────┘   └───┬───┘   └────┬────┘   └──┬──┘          │
│       │             │            │           │              │
│       └─────────────┴────────────┴───────────┘              │
│                         │                                    │
│                    ┌────┴────┐                              │
│                    │   api   │ (logs to seq)                │
│                    └────┬────┘                              │
│                         │                                    │
│                    ┌────┴────┐                              │
│                    │   web   │                              │
│                    └─────────┘                              │
│                                                              │
│  Optional (tools profile):                                   │
│  ┌───────────────────┐   ┌─────────────────┐               │
│  │ azure-data-studio │   │ redis-commander │               │
│  └───────────────────┘   └─────────────────┘               │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## Validation Rules

1. **Port Uniqueness**: All exposed ports must be unique on the host
2. **Password Strength**: SA_PASSWORD must meet SQL Server complexity requirements
3. **Volume Paths**: Source paths for bind mounts must exist
4. **Network Isolation**: All services must be on the same Docker network for inter-service communication
