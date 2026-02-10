using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.Property(e => e.ContactEmail)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(e => e.ContactPhone)
            .HasMaxLength(20);

        builder.OwnsOne(e => e.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street1)
                .IsRequired()
                .HasMaxLength(200);

            addressBuilder.Property(a => a.Street2)
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(50);

            addressBuilder.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            addressBuilder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(10);
        });

        builder.Property(e => e.SubscriptionStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(SubscriptionStatus.Trial);

        builder.Property(e => e.SubscriptionPlan)
            .HasMaxLength(50);

        builder.Property(e => e.IsPlatformOrganization)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Settings)
            .WithOne(s => s.Organization)
            .HasForeignKey<OrganizationSettings>(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrganizationSettingsConfiguration : IEntityTypeConfiguration<OrganizationSettings>
{
    public void Configure(EntityTypeBuilder<OrganizationSettings> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .IsUnique();

        builder.Property(e => e.ESignatureProvider)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ESignatureProvider.BuiltIn);

        builder.Property(e => e.AuthLinkExpirationDays)
            .IsRequired()
            .HasDefaultValue(7);

        builder.Property(e => e.NotificationEmailEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.NotificationInAppEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.NotificationSmsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DefaultTaxYearsCount)
            .IsRequired()
            .HasDefaultValue(4);

        builder.Property(e => e.Timezone)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("America/New_York");

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
