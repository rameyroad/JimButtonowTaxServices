using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_clients_organization_id");

        builder.Property(e => e.ClientType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.FirstName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(100);

        builder.HasIndex(e => new { e.OrganizationId, e.LastName, e.FirstName })
            .HasDatabaseName("ix_clients_name");

        builder.Property(e => e.BusinessName)
            .HasMaxLength(200);

        builder.HasIndex(e => new { e.OrganizationId, e.BusinessName })
            .HasDatabaseName("ix_clients_business_name");

        builder.Property(e => e.EntityType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ResponsibleParty)
            .HasMaxLength(200);

        builder.Property(e => e.TaxIdentifier)
            .IsRequired()
            .HasConversion(
                v => v.EncryptedValue,
                v => EncryptedString.FromEncrypted(v))
            .HasMaxLength(500)
            .HasColumnName("tax_identifier_encrypted");

        builder.Property(e => e.TaxIdentifierLast4)
            .IsRequired()
            .HasMaxLength(4);

        builder.HasIndex(e => new { e.OrganizationId, e.TaxIdentifierLast4 })
            .HasDatabaseName("ix_clients_tax_identifier_last4");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(e => e.Phone)
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

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        builder.Property(e => e.Version)
            .IsRequired()
            .HasDefaultValue(1)
            .IsConcurrencyToken();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Organization)
            .WithMany(o => o.Clients)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.DisplayName);
    }
}
