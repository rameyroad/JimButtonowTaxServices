using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class CaseWorkflowConfiguration : IEntityTypeConfiguration<CaseWorkflow>
{
    public void Configure(EntityTypeBuilder<CaseWorkflow> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.Property(e => e.ClientId)
            .IsRequired();

        builder.Property(e => e.WorkflowDefinitionId)
            .IsRequired();

        builder.Property(e => e.WorkflowVersion)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(WorkflowExecutionStatus.NotStarted);

        builder.Property(e => e.StartedAt);

        builder.Property(e => e.CompletedAt);

        builder.Property(e => e.StartedByUserId)
            .IsRequired();

        builder.Property(e => e.CurrentStepId);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.WorkflowDefinition)
            .WithMany()
            .HasForeignKey(e => e.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CurrentStep)
            .WithMany()
            .HasForeignKey(e => e.CurrentStepId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.StepExecutions)
            .WithOne(se => se.CaseWorkflow)
            .HasForeignKey(se => se.CaseWorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_case_workflows_organization_id");

        builder.HasIndex(e => new { e.OrganizationId, e.ClientId })
            .HasDatabaseName("ix_case_workflows_org_client");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_case_workflows_status");
    }
}

public class StepExecutionConfiguration : IEntityTypeConfiguration<StepExecution>
{
    public void Configure(EntityTypeBuilder<StepExecution> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.Property(e => e.CaseWorkflowId)
            .IsRequired();

        builder.Property(e => e.WorkflowStepId)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(StepExecutionStatus.Pending);

        builder.Property(e => e.InputData)
            .HasMaxLength(8000);

        builder.Property(e => e.OutputData)
            .HasMaxLength(8000);

        builder.Property(e => e.StartedAt);

        builder.Property(e => e.CompletedAt);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasOne(e => e.WorkflowStep)
            .WithMany()
            .HasForeignKey(e => e.WorkflowStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.CaseWorkflowId, e.WorkflowStepId })
            .HasDatabaseName("ix_step_executions_workflow_step");

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("ix_step_executions_organization_id");
    }
}
