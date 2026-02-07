using System.Security.Claims;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    private const string OrganizationIdClaimType = "org_id";
    private const string UserIdClaimType = "sub";

    // Header names for dev mode fallback
    private const string OrganizationIdHeader = "X-Organization-Id";
    private const string UserIdHeader = "X-User-Id";

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IWritableTenantContext tenantContext)
    {
        Guid? organizationId = null;
        Guid? userId = null;

        // Try to get tenant info from claims first (production auth)
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var organizationIdClaim = context.User.FindFirst(OrganizationIdClaimType)
                ?? context.User.FindFirst("https://transcript-analyzer.com/org_id");

            var userIdClaim = context.User.FindFirst(UserIdClaimType)
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (organizationIdClaim != null && Guid.TryParse(organizationIdClaim.Value, out var parsedOrgId))
            {
                organizationId = parsedOrgId;
            }

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                userId = parsedUserId;
            }
        }

        // Fallback to headers for dev mode testing
        if (!organizationId.HasValue)
        {
            if (context.Request.Headers.TryGetValue(OrganizationIdHeader, out var orgHeader) &&
                Guid.TryParse(orgHeader, out var headerOrgId))
            {
                organizationId = headerOrgId;
            }

            if (context.Request.Headers.TryGetValue(UserIdHeader, out var userHeader) &&
                Guid.TryParse(userHeader, out var headerUserId))
            {
                userId = headerUserId;
            }
        }

        if (organizationId.HasValue)
        {
            tenantContext.SetTenant(organizationId.Value, userId);

            _logger.LogDebug(
                "Tenant context set: OrganizationId={OrganizationId}, UserId={UserId}",
                organizationId,
                userId);
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
