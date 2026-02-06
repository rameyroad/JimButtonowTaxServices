using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationSettings> OrganizationSettings => Set<OrganizationSettings>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Authorization> Authorizations => Set<Authorization>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filters for multi-tenancy
        ConfigureTenantFilters(modelBuilder);
    }

    private void ConfigureTenantFilters(ModelBuilder modelBuilder)
    {
        // Only apply tenant filter when a tenant is set
        modelBuilder.Entity<User>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<Client>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<Authorization>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<Transcript>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<Notification>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<AuditLog>()
            .HasQueryFilter(e => _tenantContext.OrganizationId == null || e.OrganizationId == _tenantContext.OrganizationId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Domain.Common.BaseEntity entity)
            {
                entity.SetUpdatedAt();
            }
        }
    }
}
