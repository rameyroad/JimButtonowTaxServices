using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        // Multi-tenancy
        services.AddScoped<ITenantContext, TenantContext>();

        // Services
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
