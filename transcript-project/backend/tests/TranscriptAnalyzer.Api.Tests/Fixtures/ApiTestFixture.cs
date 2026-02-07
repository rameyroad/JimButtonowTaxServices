using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Tests.Fixtures;

public class ApiTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = null!;
    private WebApplicationFactory<Program> _factory = null!;
    public HttpClient Client { get; private set; } = null!;
    public IServiceProvider Services => _factory.Services;

    // Test tenant IDs
    public Guid TenantAId { get; } = Guid.NewGuid();
    public Guid TenantBId { get; } = Guid.NewGuid();
    public Guid UserAId { get; } = Guid.NewGuid();
    public Guid UserBId { get; } = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("transcript_analyzer_test")
            .WithUsername("postgres")
            .WithPassword("test_password")
            .Build();

        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Auth:UseDevAuth"] = "true",
                        // 256-bit (32-byte) AES key encoded as base64 for encryption service
                        ["Encryption:Key"] = "dGVzdC1lbmNyeXB0aW9uLWtleS0zMi1ieXRlcyFYWVo="
                    });
                });
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registrations
                    var descriptors = services
                        .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                                    || d.ServiceType == typeof(ApplicationDbContext))
                        .ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Add test database
                    services.AddDbContext<ApplicationDbContext>((provider, options) =>
                    {
                        options.UseNpgsql(_postgres.GetConnectionString())
                            .UseSnakeCaseNamingConvention();
                    });

                    // Build service provider to apply migrations
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                });
            });

        Client = _factory.CreateClient();

        // Seed test data
        await SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed organizations and users using raw SQL to bypass RLS
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO organizations (id, name, slug, contact_email, address_street1, address_city, address_state, address_postal_code, address_country, subscription_status, created_at, updated_at)
            VALUES
                ({0}, 'Test Org A', 'test-org-a', 'admin@orga.com', '123 Main St', 'New York', 'NY', '10001', 'US', 'Active', {2}, {2}),
                ({1}, 'Test Org B', 'test-org-b', 'admin@orgb.com', '456 Oak Ave', 'Los Angeles', 'CA', '90001', 'US', 'Active', {2}, {2})",
            TenantAId, TenantBId, DateTime.UtcNow);

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO users (id, organization_id, auth0user_id, email, first_name, last_name, role, status, created_at, updated_at)
            VALUES
                ({0}, {2}, 'auth0|usera', 'user@orga.com', 'User', 'A', 'Admin', 'Active', {4}, {4}),
                ({1}, {3}, 'auth0|userb', 'user@orgb.com', 'User', 'B', 'Admin', 'Active', {4}, {4})",
            UserAId, UserBId, TenantAId, TenantBId, DateTime.UtcNow);
    }

    public HttpClient CreateClientWithTenant(Guid organizationId, Guid userId, string role = "Admin")
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Organization-Id", organizationId.ToString());
        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        client.DefaultRequestHeaders.Add("X-User-Role", role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "dev-token");
        return client;
    }

    public HttpClient CreateUnauthenticatedClient()
    {
        return _factory.CreateClient();
    }
}

[CollectionDefinition("Api")]
public class ApiTestCollection : ICollectionFixture<ApiTestFixture>
{
}
