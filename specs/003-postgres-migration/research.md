# Research: PostgreSQL Migration

**Date**: 2026-02-06
**Branch**: `003-postgres-migration`

## Research Topics

1. EF Core snake_case naming conventions
2. PostgreSQL Row-Level Security with EF Core
3. Connection pooling with tenant context
4. Azure PostgreSQL Flexible Server configuration

---

## 1. Snake_case Naming Convention

### Decision
Use `EFCore.NamingConventions` package with `.UseSnakeCaseNamingConvention()`.

### Rationale
- Official community package maintained alongside EF Core
- Automatically converts tables, columns, indexes, and constraints
- PostgreSQL treats unquoted identifiers as lowercase, making snake_case the natural choice
- Avoids need for manual `ToTable()` and `HasColumnName()` calls in every configuration

### Configuration

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());
```

### Package Versions
- `EFCore.NamingConventions` 9.0.x (matches EF Core 9.0)
- `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.x

### Alternatives Considered
1. **Manual naming in each configuration** - Rejected: High maintenance burden, error-prone
2. **Custom IModelCustomizer** - Rejected: More complex than proven package solution
3. **EF Core `UseHiLo` with custom conventions** - Rejected: Overkill for naming only

---

## 2. Row-Level Security Implementation

### Decision
Implement RLS via EF Core migrations using raw SQL, with DbConnectionInterceptor for tenant context.

### Rationale
- RLS policies are DDL and must be in migrations for version control
- DbConnectionInterceptor provides hook at connection acquisition (before any queries)
- Single roundtrip per request (set tenant once when connection opens)
- Defense-in-depth: RLS + application query filters

### RLS Policy Pattern

```sql
-- Enable RLS on table
ALTER TABLE clients ENABLE ROW LEVEL SECURITY;

-- Create policy for tenant isolation
CREATE POLICY tenant_isolation ON clients
    FOR ALL
    TO app_user
    USING (organization_id = NULLIF(current_setting('app.organization_id', TRUE), '')::uuid);
```

### Connection Interceptor Pattern

```csharp
public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ITenantContext _tenantContext;

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        // Open connection first
        await ((NpgsqlConnection)connection).OpenAsync(cancellationToken);

        // Reset and set tenant context
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = _tenantContext.OrganizationId.HasValue
            ? $"SELECT set_config('app.organization_id', '{_tenantContext.OrganizationId}', false)"
            : "RESET app.organization_id";
        await cmd.ExecuteNonQueryAsync(cancellationToken);

        return InterceptionResult.Suppress(); // We already opened the connection
    }
}
```

### Database Role Structure

| Role | RLS Bypass | Purpose |
|------|------------|---------|
| `app_user` | No | Application connection (standard operations) |
| `app_admin` | Yes | Admin connection (maintenance, cross-tenant reports) |

### Tables Requiring RLS
- `clients`
- `authorizations`
- `transcripts`
- `users`
- `notifications`
- `audit_logs`

### Tables NOT Requiring RLS
- `organizations` (anchor entity - users access their own org by design)
- `organization_settings` (accessed via organization relationship)

### Alternatives Considered
1. **Application-only query filters** - Rejected: Can be bypassed with raw SQL
2. **Separate database per tenant** - Rejected: Operational complexity, cost prohibitive
3. **Schema-per-tenant** - Rejected: Migration complexity, connection string changes

---

## 3. Connection Pooling with Tenant Context

### Decision
Reset and set tenant context on every connection acquisition; clear on connection return.

### Rationale
- Pooled connections may retain session state from previous requests
- Security requires guaranteed fresh context for each tenant
- Small performance overhead (one SET command) is acceptable for security

### Connection Lifecycle

```text
Request Start
    │
    ▼
┌─────────────────────────────────────┐
│ Acquire connection from pool        │
│ Execute: RESET app.organization_id  │
│ Execute: SET app.organization_id    │
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│ Execute queries (RLS applied)       │
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│ Return connection to pool           │
│ (Context cleared by next acquire)   │
└─────────────────────────────────────┘
```

### Connection String Parameters

```text
Pooling=true
Maximum Pool Size=100
Minimum Pool Size=5
Connection Idle Lifetime=300
```

### Alternatives Considered
1. **Clear context on connection return** - Hybrid approach, adds complexity
2. **Disable pooling** - Rejected: Unacceptable performance impact
3. **Per-tenant connection pools** - Rejected: Resource intensive, doesn't scale

---

## 4. Azure PostgreSQL Flexible Server

### Decision
Use standard Npgsql connection string with SSL required; no code changes for Azure deployment.

### Rationale
- Azure PostgreSQL uses standard PostgreSQL protocol
- SSL is enforced by Azure; `SSLMode=Require` ensures compliance
- Connection retry handles transient Azure networking issues

### Connection String Format

**Local Development:**
```text
Host=localhost;Port=5432;Database=transcript_analyzer;Username=postgres;Password=xxx
```

**Azure Production:**
```text
Host=myserver.postgres.database.azure.com;Port=5432;Database=transcript_analyzer;Username=appuser;Password=xxx;SSLMode=Require
```

### Azure-Specific Configuration

```csharp
options.UseNpgsql(connectionString, npgsqlOptions =>
{
    npgsqlOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorCodesToAdd: null);
    npgsqlOptions.CommandTimeout(30);
});
```

### Azure Limitations
- No client certificate authentication (mutual TLS not supported)
- Must use password or Azure AD authentication
- `sslcert` and `sslkey` parameters not applicable

### Alternatives Considered
1. **Azure SQL** - Already rejected in favor of PostgreSQL for RLS and cost
2. **Self-hosted PostgreSQL on Azure VM** - Rejected: More operational overhead

---

## Summary of Technical Decisions

| Area | Decision | Package/Config |
|------|----------|----------------|
| Naming | Snake_case via convention | `EFCore.NamingConventions` 9.0.x |
| RLS Creation | Raw SQL in migrations | `MigrationBuilder.Sql()` |
| Tenant Context | DbConnectionInterceptor | Custom `TenantConnectionInterceptor` |
| Context Reset | On every connection acquire | `RESET` then `SET` pattern |
| Admin Bypass | Separate connection string | `app_admin` role with RLS bypass |
| Azure SSL | Required | `SSLMode=Require` |
| Retry Logic | Built-in Npgsql | `EnableRetryOnFailure(3, 30s)` |

---

## References

- [EFCore.NamingConventions GitHub](https://github.com/efcore/EFCore.NamingConventions)
- [PostgreSQL RLS Documentation](https://www.postgresql.org/docs/16/ddl-rowsecurity.html)
- [Npgsql Connection Pooling](https://www.npgsql.org/doc/connection-pooling.html)
- [Azure PostgreSQL TLS Configuration](https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/how-to-connect-tls-ssl)
- [EF Core Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors)
