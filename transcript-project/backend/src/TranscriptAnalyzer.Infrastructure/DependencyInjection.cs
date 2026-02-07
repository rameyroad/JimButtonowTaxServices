using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Domain.Interfaces;
using TranscriptAnalyzer.Infrastructure.Persistence;
using TranscriptAnalyzer.Infrastructure.Security;
using TranscriptAnalyzer.Infrastructure.Storage;

namespace TranscriptAnalyzer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Multi-tenancy - register first so it's available for the interceptor
        // Register as singleton instance so the same context is shared across both interfaces
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<IWritableTenantContext>(sp => sp.GetRequiredService<TenantContext>());

        // Register the tenant connection interceptor
        services.AddScoped<TenantConnectionInterceptor>();

        // Database - PostgreSQL with snake_case naming convention and RLS support
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var tenantInterceptor = serviceProvider.GetRequiredService<TenantConnectionInterceptor>();

            options
                .UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    })
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(tenantInterceptor);
        });

        // Register DbContext as abstract type for Application layer handlers
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Admin DbContext factory for RLS bypass (maintenance operations)
        // Uses AdminConnection which connects as app_admin role
        var adminConnectionString = configuration.GetConnectionString("AdminConnection");
        if (!string.IsNullOrEmpty(adminConnectionString))
        {
            services.AddDbContextFactory<ApplicationDbContext>(
                options => options
                    .UseNpgsql(adminConnectionString)
                    .UseSnakeCaseNamingConvention(),
                ServiceLifetime.Scoped);
        }

        // Services
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
