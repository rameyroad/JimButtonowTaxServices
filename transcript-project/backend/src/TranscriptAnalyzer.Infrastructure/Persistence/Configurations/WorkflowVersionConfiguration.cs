using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class WorkflowVersionConfiguration : IEntityTypeConfiguration<WorkflowVersion>
{
    public void Configure(EntityTypeBuilder<WorkflowVersion> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.WorkflowDefinitionId)
            .IsRequired();

        builder.Property(e => e.VersionNumber)
            .IsRequired();

        builder.Property(e => e.PublishedAt)
            .IsRequired();

        builder.Property(e => e.PublishedByUserId)
            .IsRequired();

        builder.Property(e => e.SnapshotData)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.WorkflowDefinition)
            .WithMany(wd => wd.Versions)
            .HasForeignKey(e => e.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.WorkflowDefinitionId, e.VersionNumber })
            .IsUnique()
            .HasDatabaseName("ix_workflow_versions_definition_version");

        builder.HasIndex(e => new { e.WorkflowDefinitionId, e.IsActive })
            .HasDatabaseName("ix_workflow_versions_definition_active");
    }
}
