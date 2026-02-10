using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class ClientApprovalConfiguration : IEntityTypeConfiguration<ClientApproval>
{
    public void Configure(EntityTypeBuilder<ClientApproval> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_client_approvals_organization_id");

        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("ix_client_approvals_token");

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.CaseWorkflowId)
            .HasDatabaseName("ix_client_approvals_case_workflow_id");

        builder.HasIndex(e => e.ClientId)
            .HasDatabaseName("ix_client_approvals_client_id");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.TokenExpiresAt)
            .IsRequired();

        builder.Property(e => e.ResponseNotes)
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.CaseWorkflow)
            .WithMany()
            .HasForeignKey(e => e.CaseWorkflowId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.StepExecution)
            .WithMany()
            .HasForeignKey(e => e.StepExecutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
