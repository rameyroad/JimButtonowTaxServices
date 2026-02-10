using System.Security.Claims;
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

    // Suppress security warnings for dev auth - this is intentionally insecure for local development
#pragma warning disable CA5404
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

                // In dev mode, always create a valid principal even with invalid tokens
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Create a valid identity anyway for dev testing
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, "dev-user"),
                            new(ClaimTypes.NameIdentifier, "dev-user-id")
                        };
                        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                        context.Principal = new ClaimsPrincipal(identity);
                        context.Success();
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = _ => Task.CompletedTask
                };
            });
#pragma warning restore CA5404

        services.AddAuthorization(options =>
        {
            // Default policy allows all authenticated requests
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();

            // WriteClients policy - requires Admin or TaxProfessional role
            options.AddPolicy("WriteClients", policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    if (httpContext == null) return true; // Allow if no HttpContext

                    var role = httpContext.Request.Headers["X-User-Role"].FirstOrDefault();
                    return role != "ReadOnly"; // All roles except ReadOnly can write
                }));

            // AdminOnly policy - requires Admin role
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    if (httpContext == null) return true; // Allow if no HttpContext

                    var role = httpContext.Request.Headers["X-User-Role"].FirstOrDefault();
                    return role == "Admin"; // Only Admin role allowed
                }));

            // PlatformAdmin policy - requires PlatformAdmin role
            options.AddPolicy("PlatformAdmin", policy =>
                policy.RequireAssertion(context =>
                {
                    var httpContext = context.Resource as HttpContext;
                    if (httpContext == null) return true; // Allow if no HttpContext

                    var role = httpContext.Request.Headers["X-User-Role"].FirstOrDefault();
                    return role == "PlatformAdmin";
                }));
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

            // AdminOnly policy for archive/restore operations
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("permissions", "admin:clients"));

            // PlatformAdmin policy for platform-level operations
            options.AddPolicy("PlatformAdmin", policy =>
                policy.RequireClaim("permissions", "platform:admin"));
        });

        return services;
    }
}
