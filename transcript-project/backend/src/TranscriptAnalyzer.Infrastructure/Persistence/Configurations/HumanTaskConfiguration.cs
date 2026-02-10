using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class HumanTaskConfiguration : IEntityTypeConfiguration<HumanTask>
{
    public void Configure(EntityTypeBuilder<HumanTask> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_human_tasks_organization_id");

        builder.Property(e => e.CaseWorkflowId)
            .IsRequired();

        builder.HasIndex(e => e.CaseWorkflowId)
            .HasDatabaseName("ix_human_tasks_case_workflow_id");

        builder.HasIndex(e => e.StepExecutionId)
            .HasDatabaseName("ix_human_tasks_step_execution_id");

        builder.HasIndex(e => new { e.AssignedToUserId, e.Status })
            .HasDatabaseName("ix_human_tasks_assignee_status");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Decision)
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
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
    }
}
