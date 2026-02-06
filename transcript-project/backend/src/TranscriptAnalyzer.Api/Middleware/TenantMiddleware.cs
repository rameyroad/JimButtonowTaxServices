using System.Security.Claims;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    private const string OrganizationIdClaimType = "org_id";
    private const string UserIdClaimType = "sub";

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var organizationIdClaim = context.User.FindFirst(OrganizationIdClaimType)
                ?? context.User.FindFirst("https://transcript-analyzer.com/org_id");

            var userIdClaim = context.User.FindFirst(UserIdClaimType)
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (organizationIdClaim != null && Guid.TryParse(organizationIdClaim.Value, out var organizationId))
            {
                Guid? userId = null;
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                tenantContext.SetTenant(organizationId, userId);

                _logger.LogDebug(
                    "Tenant context set: OrganizationId={OrganizationId}, UserId={UserId}",
                    organizationId,
                    userId);
            }
            else
            {
                _logger.LogWarning(
                    "Authenticated user without organization claim: {Subject}",
                    userIdClaim?.Value);
            }
        }

        try
        {
            await _next(context);
        }
        finally
        {
            tenantContext.Clear();
        }
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
