using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class AuthorizationConfiguration : IEntityTypeConfiguration<Authorization>
{
    public void Configure(EntityTypeBuilder<Authorization> builder)
    {
        builder.ToTable("Authorizations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("IX_Authorizations_OrganizationId");

        builder.Property(e => e.ClientId)
            .IsRequired();

        builder.HasIndex(e => e.ClientId)
            .HasDatabaseName("IX_Authorizations_ClientId");

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(AuthorizationStatus.Draft);

        builder.HasIndex(e => new { e.OrganizationId, e.Status })
            .HasDatabaseName("IX_Authorizations_Status");

        builder.Property<List<int>>("_taxYears")
            .HasColumnName("TaxYears")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>());

        builder.Property(e => e.SignatureRequestToken)
            .HasMaxLength(64);

        builder.Property(e => e.SignatureRequestExpiresAt);

        builder.Property(e => e.SignatureData);

        builder.Property(e => e.SignedAt);

        builder.Property(e => e.SignedByIp)
            .HasMaxLength(45);

        builder.Property(e => e.SignedByUserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.ExternalSignatureId)
            .HasMaxLength(100);

        builder.Property(e => e.FormBlobPath)
            .HasMaxLength(500);

        builder.Property(e => e.ExpirationDate);

        builder.HasIndex(e => e.ExpirationDate)
            .HasDatabaseName("IX_Authorizations_ExpirationDate")
            .HasFilter("[Status] = 'Active'");

        builder.Property(e => e.CafSubmissionDate);

        builder.Property(e => e.CafConfirmationDate);

        builder.Property(e => e.CafReferenceNumber)
            .HasMaxLength(50);

        builder.Property(e => e.RevokedAt);

        builder.Property(e => e.RevokedReason)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Client)
            .WithMany(c => c.Authorizations)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.TaxYears);
        builder.Ignore(e => e.IsActive);
        builder.Ignore(e => e.DaysUntilExpiration);
    }
}
