using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TranscriptAnalyzer.Api.Configuration;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useDevAuth = configuration.GetValue<bool>("Auth:UseDevAuth");

        if (useDevAuth)
        {
            return services.AddDevAuthentication();
        }

        return services.AddAuth0Authentication(configuration);
    }

    private static IServiceCollection AddDevAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    RequireSignedTokens = false
                };
            });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();
        });

        return services;
    }

    private static IServiceCollection AddAuth0Authentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var domain = configuration["Auth0:Domain"]
            ?? throw new InvalidOperationException("Auth0:Domain configuration is required.");
        var audience = configuration["Auth0:Audience"]
            ?? throw new InvalidOperationException("Auth0:Audience configuration is required.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{domain}/";
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://{domain}/",
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });

        services.AddAuthorization(options =>
        {
            // Permission-based policies
            options.AddPolicy("ReadClients", policy =>
                policy.RequireClaim("permissions", "read:clients"));
            options.AddPolicy("WriteClients", policy =>
                policy.RequireClaim("permissions", "write:clients"));

            options.AddPolicy("ReadAuthorizations", policy =>
                policy.RequireClaim("permissions", "read:authorizations"));
            options.AddPolicy("WriteAuthorizations", policy =>
                policy.RequireClaim("permissions", "write:authorizations"));

            options.AddPolicy("ReadTranscripts", policy =>
                policy.RequireClaim("permissions", "read:transcripts"));
            options.AddPolicy("WriteTranscripts", policy =>
                policy.RequireClaim("permissions", "write:transcripts"));

            options.AddPolicy("ManageOrganization", policy =>
                policy.RequireClaim("permissions", "manage:organization"));
        });

        return services;
    }
}
