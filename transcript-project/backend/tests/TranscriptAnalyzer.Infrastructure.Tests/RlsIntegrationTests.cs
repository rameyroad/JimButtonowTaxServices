using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Infrastructure.Tests;

/// <summary>
/// Integration tests for Row-Level Security (RLS) policies.
/// These tests verify that tenant isolation is enforced at the database level.
/// </summary>
public class RlsIntegrationTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = null!;
    private string _connectionString = null!;

    // Test organization IDs
    private readonly Guid _tenantAId = Guid.NewGuid();
    private readonly Guid _tenantBId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("transcript_analyzer_test")
            .WithUsername("postgres")
            .WithPassword("test_password")
            .Build();

        await _postgres.StartAsync();
        _connectionString = _postgres.GetConnectionString();

        // Create schema and apply RLS policies
        await SetupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    private async Task SetupDatabaseAsync()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention();

        // Create a tenant context that doesn't filter (for setup)
        var tenantContext = new TestTenantContext();

        await using var context = new ApplicationDbContext(optionsBuilder.Options, tenantContext);

        // Create the database schema
        await context.Database.MigrateAsync();

        // Grant app_user role to postgres so we can SET ROLE for testing
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("GRANT app_user TO postgres", conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task SeedTestDataAsync()
    {
        // Use raw SQL to insert test data (bypassing RLS for setup)
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Create organizations
        await using (var cmd = new NpgsqlCommand(@"
            INSERT INTO organizations (id, name, slug, contact_email, address_street1, address_city, address_state, address_postal_code, address_country, subscription_status, created_at, updated_at)
            VALUES
                (@idA, 'Tenant A', 'tenant-a', 'admin@tenanta.com', '123 Main St', 'New York', 'NY', '10001', 'US', 'Active', @created, @updated),
                (@idB, 'Tenant B', 'tenant-b', 'admin@tenantb.com', '456 Oak Ave', 'Los Angeles', 'CA', '90001', 'US', 'Active', @created, @updated)", conn))
        {
            cmd.Parameters.AddWithValue("idA", _tenantAId);
            cmd.Parameters.AddWithValue("idB", _tenantBId);
            cmd.Parameters.AddWithValue("created", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("updated", DateTime.UtcNow);
            await cmd.ExecuteNonQueryAsync();
        }

        // Create users for each tenant
        await using (var cmd = new NpgsqlCommand(@"
            INSERT INTO users (id, organization_id, auth0user_id, email, first_name, last_name, role, status, created_at, updated_at)
            VALUES
                (@userAId, @tenantAId, 'auth0|usera', 'user@tenanta.com', 'User', 'A', 'Admin', 'Active', @created, @updated),
                (@userBId, @tenantBId, 'auth0|userb', 'user@tenantb.com', 'User', 'B', 'Admin', 'Active', @created, @updated)", conn))
        {
            cmd.Parameters.AddWithValue("userAId", Guid.NewGuid());
            cmd.Parameters.AddWithValue("userBId", Guid.NewGuid());
            cmd.Parameters.AddWithValue("tenantAId", _tenantAId);
            cmd.Parameters.AddWithValue("tenantBId", _tenantBId);
            cmd.Parameters.AddWithValue("created", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("updated", DateTime.UtcNow);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task<int> QueryUsersWithTenantContextAsync(Guid? tenantId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Switch to app_user role so RLS is enforced (postgres superuser bypasses RLS)
        await using (var roleCmd = new NpgsqlCommand("SET ROLE app_user", conn))
        {
            await roleCmd.ExecuteNonQueryAsync();
        }

        // Set the tenant context in PostgreSQL session
        var tenantIdValue = tenantId?.ToString() ?? "";
        await using (var setCmd = new NpgsqlCommand(
            "SELECT set_config('app.current_tenant_id', @tenantId, false)", conn))
        {
            setCmd.Parameters.AddWithValue("tenantId", tenantIdValue);
            await setCmd.ExecuteNonQueryAsync();
        }

        // Query users - RLS should filter based on tenant context
        await using var queryCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users", conn);
        var result = await queryCmd.ExecuteScalarAsync();
        return Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture);
    }

    private async Task<List<string>> QueryUserEmailsWithTenantContextAsync(Guid tenantId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Switch to app_user role so RLS is enforced (postgres superuser bypasses RLS)
        await using (var roleCmd = new NpgsqlCommand("SET ROLE app_user", conn))
        {
            await roleCmd.ExecuteNonQueryAsync();
        }

        // Set the tenant context
        await using (var setCmd = new NpgsqlCommand(
            "SELECT set_config('app.current_tenant_id', @tenantId, false)", conn))
        {
            setCmd.Parameters.AddWithValue("tenantId", tenantId.ToString());
            await setCmd.ExecuteNonQueryAsync();
        }

        // Query users
        var emails = new List<string>();
        await using var queryCmd = new NpgsqlCommand("SELECT email FROM users", conn);
        await using var reader = await queryCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            emails.Add(reader.GetString(0));
        }
        return emails;
    }

    [Fact]
    public async Task Query_WithoutTenantContext_ReturnsNoRows()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act - Query without setting tenant context (empty string)
        var count = await QueryUsersWithTenantContextAsync(null);

        // Assert - RLS with empty tenant should return no rows
        count.Should().Be(0);
    }

    [Fact]
    public async Task Query_WithTenantAContext_ReturnsOnlyTenantAData()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act - Query with Tenant A context
        var emails = await QueryUserEmailsWithTenantContextAsync(_tenantAId);

        // Assert - Should only see Tenant A's user
        emails.Should().HaveCount(1);
        emails[0].Should().Be("user@tenanta.com");
    }

    [Fact]
    public async Task Query_WithTenantBContext_ReturnsOnlyTenantBData()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act - Query with Tenant B context
        var emails = await QueryUserEmailsWithTenantContextAsync(_tenantBId);

        // Assert - Should only see Tenant B's user
        emails.Should().HaveCount(1);
        emails[0].Should().Be("user@tenantb.com");
    }

    [Fact]
    public async Task Insert_WithWrongTenantContext_IsRejected()
    {
        // Arrange
        await SeedTestDataAsync();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Switch to app_user role so RLS is enforced (postgres superuser bypasses RLS)
        await using (var roleCmd = new NpgsqlCommand("SET ROLE app_user", conn))
        {
            await roleCmd.ExecuteNonQueryAsync();
        }

        // Set tenant context to Tenant B
        await using (var setCmd = new NpgsqlCommand(
            "SELECT set_config('app.current_tenant_id', @tenantId, false)", conn))
        {
            setCmd.Parameters.AddWithValue("tenantId", _tenantBId.ToString());
            await setCmd.ExecuteNonQueryAsync();
        }

        // Act & Assert - Try to insert a user for Tenant A while context is Tenant B
        // RLS WITH CHECK should prevent this
        var act = async () =>
        {
            await using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO users (id, organization_id, auth0user_id, email, first_name, last_name, role, status, created_at, updated_at)
                VALUES (@id, @orgId, 'auth0|hacker', 'hacker@tenanta.com', 'Hacker', 'User', 'Admin', 'Active', @created, @updated)", conn);
            insertCmd.Parameters.AddWithValue("id", Guid.NewGuid());
            insertCmd.Parameters.AddWithValue("orgId", _tenantAId); // Wrong tenant!
            insertCmd.Parameters.AddWithValue("created", DateTime.UtcNow);
            insertCmd.Parameters.AddWithValue("updated", DateTime.UtcNow);
            await insertCmd.ExecuteNonQueryAsync();
        };

        // This should throw a PostgresException due to RLS policy violation
        await act.Should().ThrowAsync<PostgresException>()
            .Where(ex => ex.SqlState == "42501"); // Insufficient privilege
    }

    [Fact]
    public async Task Query_WithAppAdminRole_BypassesRls()
    {
        // Arrange
        await SeedTestDataAsync();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Grant app_admin role to postgres
        await using (var grantCmd = new NpgsqlCommand("GRANT app_admin TO postgres", conn))
        {
            await grantCmd.ExecuteNonQueryAsync();
        }

        // Switch to app_admin role which has BYPASSRLS
        await using (var roleCmd = new NpgsqlCommand("SET ROLE app_admin", conn))
        {
            await roleCmd.ExecuteNonQueryAsync();
        }

        // Do NOT set tenant context - app_admin should bypass RLS and see all rows

        // Act - Query users without tenant context as app_admin
        await using var queryCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users", conn);
        var result = await queryCmd.ExecuteScalarAsync();
        var count = Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture);

        // Assert - app_admin should see all users from both tenants
        count.Should().Be(2);
    }

    /// <summary>
    /// Test tenant context for RLS tests.
    /// </summary>
    private sealed class TestTenantContext : ITenantContext
    {
        private Guid? _organizationId;
        private Guid? _userId;

        public Guid? OrganizationId => _organizationId;
        public Guid? UserId => _userId;

        public void SetTenant(Guid organizationId, Guid? userId = null)
        {
            _organizationId = organizationId;
            _userId = userId;
        }

        public void Clear()
        {
            _organizationId = null;
            _userId = null;
        }
    }
}
