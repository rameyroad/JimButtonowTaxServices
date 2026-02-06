using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class OrganizationSettings : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public ESignatureProvider ESignatureProvider { get; private set; }
    public int AuthLinkExpirationDays { get; private set; }
    public bool NotificationEmailEnabled { get; private set; }
    public bool NotificationInAppEnabled { get; private set; }
    public bool NotificationSmsEnabled { get; private set; }
    public int DefaultTaxYearsCount { get; private set; }
    public string Timezone { get; private set; }

    public Organization? Organization { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private OrganizationSettings() { }
#pragma warning restore CS8618

    public OrganizationSettings(Guid organizationId)
    {
        OrganizationId = organizationId;
        ESignatureProvider = ESignatureProvider.BuiltIn;
        AuthLinkExpirationDays = 7;
        NotificationEmailEnabled = true;
        NotificationInAppEnabled = true;
        NotificationSmsEnabled = false;
        DefaultTaxYearsCount = 4;
        Timezone = "America/New_York";
    }

    public void UpdateESignatureProvider(ESignatureProvider provider)
    {
        ESignatureProvider = provider;
        SetUpdatedAt();
    }

    public void UpdateAuthLinkExpiration(int days)
    {
        if (days < 1 || days > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "Auth link expiration must be between 1 and 30 days.");
        }

        AuthLinkExpirationDays = days;
        SetUpdatedAt();
    }

    public void UpdateNotificationSettings(bool emailEnabled, bool inAppEnabled, bool smsEnabled)
    {
        NotificationEmailEnabled = emailEnabled;
        NotificationInAppEnabled = inAppEnabled;
        NotificationSmsEnabled = smsEnabled;
        SetUpdatedAt();
    }

    public void UpdateDefaultTaxYearsCount(int count)
    {
        if (count < 1 || count > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Default tax years count must be between 1 and 4.");
        }

        DefaultTaxYearsCount = count;
        SetUpdatedAt();
    }

    public void UpdateTimezone(string timezone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone);
        Timezone = timezone;
        SetUpdatedAt();
    }
}
