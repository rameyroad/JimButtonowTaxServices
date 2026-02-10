using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api;

public static class DevDataSeeder
{
    // Well-known dev IDs — must match frontend baseApi.ts
    public static readonly Guid DevOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid DevUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");

    public static async Task SeedDevDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var orgExists = await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO organizations (id, name, slug, contact_email, address_street1, address_city, address_state, address_postal_code, address_country, subscription_status, created_at, updated_at)
            SELECT {0}, 'Dev Organization', 'dev-org', 'admin@dev.local', '123 Main St', 'New York', 'NY', '10001', 'US', 'Active', {1}, {1}
            WHERE NOT EXISTS (SELECT 1 FROM organizations WHERE id = {0})",
            DevOrganizationId, DateTime.UtcNow);

        var userExists = await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO users (id, organization_id, auth0user_id, email, first_name, last_name, role, status, created_at, updated_at)
            SELECT {0}, {1}, 'auth0|dev-user', 'jim@buttonow.com', 'Jim', 'Buttonow', 'Admin', 'Active', {2}, {2}
            WHERE NOT EXISTS (SELECT 1 FROM users WHERE id = {0})",
            DevUserId, DevOrganizationId, DateTime.UtcNow);

        logger.LogInformation(
            "Dev data seeded: OrganizationId={OrganizationId}, UserId={UserId}",
            DevOrganizationId, DevUserId);
    }
}
