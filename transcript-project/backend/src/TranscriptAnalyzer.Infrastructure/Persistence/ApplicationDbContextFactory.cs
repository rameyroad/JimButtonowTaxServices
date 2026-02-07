using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TranscriptAnalyzer.Application.Common;

namespace TranscriptAnalyzer.Infrastructure.Persistence;

/// <summary>
/// Factory for creating ApplicationDbContext at design time (for EF Core migrations).
/// This factory is automatically discovered by the EF Core tools.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a default PostgreSQL connection string for design-time operations
        // This is only used for generating migrations, not for production
        optionsBuilder
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=transcript_analyzer;Username=postgres;Password=YourStrong@Passw0rd",
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                })
            .UseSnakeCaseNamingConvention();

        // Use a design-time tenant context that doesn't filter anything
        return new ApplicationDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    /// <summary>
    /// Design-time tenant context with no organization set (no filtering).
    /// </summary>
    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? OrganizationId => null;
        public Guid? UserId => null;
    }
}
