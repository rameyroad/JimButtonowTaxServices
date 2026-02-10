using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PublishStatus.Draft);

        builder.Property(e => e.CurrentVersion)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.PublishedByUserId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasMany(e => e.Steps)
            .WithOne(s => s.WorkflowDefinition)
            .HasForeignKey(s => s.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("ix_workflow_definitions_name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_workflow_definitions_status");

        builder.HasIndex(e => e.Category)
            .HasDatabaseName("ix_workflow_definitions_category");
    }
}

public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.WorkflowDefinitionId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.StepType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.Property(e => e.Configuration)
            .HasMaxLength(4000);

        builder.Property(e => e.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.NextStepOnSuccess)
            .WithMany()
            .HasForeignKey(e => e.NextStepOnSuccessId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.NextStepOnFailure)
            .WithMany()
            .HasForeignKey(e => e.NextStepOnFailureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.WorkflowDefinitionId, e.SortOrder })
            .HasDatabaseName("ix_workflow_steps_definition_sort");
    }
}
