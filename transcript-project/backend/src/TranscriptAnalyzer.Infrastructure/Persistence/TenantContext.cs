using TranscriptAnalyzer.Application.Common;

namespace TranscriptAnalyzer.Infrastructure.Persistence;

public interface IWritableTenantContext : ITenantContext
{
    void SetTenant(Guid organizationId, Guid? userId = null);
    void Clear();
}

public sealed class TenantContext : IWritableTenantContext
{
    private readonly AsyncLocal<Guid?> _organizationId = new();
    private readonly AsyncLocal<Guid?> _userId = new();

    public Guid? OrganizationId => _organizationId.Value;
    public Guid? UserId => _userId.Value;

    public void SetTenant(Guid organizationId, Guid? userId = null)
    {
        _organizationId.Value = organizationId;
        _userId.Value = userId;
    }

    public void Clear()
    {
        _organizationId.Value = null;
        _userId.Value = null;
    }
}
