namespace TranscriptAnalyzer.Application.Common;

/// <summary>
/// Provides access to the current tenant context for multi-tenant operations.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current organization (tenant) ID.
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    Guid? UserId { get; }
}
