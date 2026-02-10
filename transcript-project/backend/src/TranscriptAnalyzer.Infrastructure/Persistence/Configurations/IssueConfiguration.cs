using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_issues_organization_id");

        builder.Property(e => e.ClientId)
            .IsRequired();

        builder.HasIndex(e => e.ClientId)
            .HasDatabaseName("ix_issues_client_id");

        builder.HasIndex(e => new { e.ClientId, e.TaxYear })
            .HasDatabaseName("ix_issues_client_tax_year");

        builder.Property(e => e.IssueType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.TaxYear)
            .IsRequired();

        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.TransactionCode)
            .HasMaxLength(10);

        builder.Property(e => e.DetectedAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CaseWorkflow)
            .WithMany()
            .HasForeignKey(e => e.CaseWorkflowId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
