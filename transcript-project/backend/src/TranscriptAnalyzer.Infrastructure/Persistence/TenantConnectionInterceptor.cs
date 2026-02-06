using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TranscriptAnalyzer.Infrastructure.Persistence;

/// <summary>
/// EF Core interceptor that sets the PostgreSQL session variable for tenant context
/// on every connection open. This enables Row-Level Security (RLS) policies to
/// filter data based on the current tenant.
/// </summary>
public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ITenantContext _tenantContext;

    public TenantConnectionInterceptor(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenantContext(connection);
        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await SetTenantContextAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void SetTenantContext(DbConnection connection)
    {
        // Always reset the tenant context first to ensure clean state
        // This is critical for connection pooling security
        using var resetCommand = connection.CreateCommand();
        resetCommand.CommandText = "SELECT set_config('app.current_tenant_id', '', false)";
        resetCommand.ExecuteNonQuery();

        // Set the tenant context if we have one
        if (_tenantContext.OrganizationId.HasValue)
        {
            using var setCommand = connection.CreateCommand();
            // Use parameterized query - Guid.ToString() is safe but we use parameter for consistency
            setCommand.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, false)";
            var param = setCommand.CreateParameter();
            param.ParameterName = "@tenantId";
            param.Value = _tenantContext.OrganizationId.Value.ToString();
            setCommand.Parameters.Add(param);
            setCommand.ExecuteNonQuery();
        }
    }

    private async Task SetTenantContextAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        // Always reset the tenant context first to ensure clean state
        // This is critical for connection pooling security
        await using var resetCommand = connection.CreateCommand();
        resetCommand.CommandText = "SELECT set_config('app.current_tenant_id', '', false)";
        await resetCommand.ExecuteNonQueryAsync(cancellationToken);

        // Set the tenant context if we have one
        if (_tenantContext.OrganizationId.HasValue)
        {
            await using var setCommand = connection.CreateCommand();
            // Use parameterized query - Guid.ToString() is safe but we use parameter for consistency
            setCommand.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, false)";
            var param = setCommand.CreateParameter();
            param.ParameterName = "@tenantId";
            param.Value = _tenantContext.OrganizationId.Value.ToString();
            setCommand.Parameters.Add(param);
            await setCommand.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
