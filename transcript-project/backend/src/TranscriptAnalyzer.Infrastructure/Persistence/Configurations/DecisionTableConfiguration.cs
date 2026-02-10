using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class DecisionTableConfiguration : IEntityTypeConfiguration<DecisionTable>
{
    public void Configure(EntityTypeBuilder<DecisionTable> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PublishStatus.Draft);

        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.PublishedByUserId);

        builder.Property(e => e.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasMany(e => e.Columns)
            .WithOne(c => c.DecisionTable)
            .HasForeignKey(c => c.DecisionTableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Rules)
            .WithOne(r => r.DecisionTable)
            .HasForeignKey(r => r.DecisionTableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("ix_decision_tables_name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_decision_tables_status");
    }
}

public class DecisionTableColumnConfiguration : IEntityTypeConfiguration<DecisionTableColumn>
{
    public void Configure(EntityTypeBuilder<DecisionTableColumn> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DecisionTableId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DataType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.IsInput)
            .IsRequired();

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasIndex(e => new { e.DecisionTableId, e.Key })
            .IsUnique()
            .HasDatabaseName("ix_decision_table_columns_table_key");
    }
}

public class DecisionRuleConfiguration : IEntityTypeConfiguration<DecisionRule>
{
    public void Configure(EntityTypeBuilder<DecisionRule> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DecisionTableId)
            .IsRequired();

        builder.Property(e => e.Priority)
            .IsRequired();

        builder.Property(e => e.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasMany(e => e.Conditions)
            .WithOne(c => c.DecisionRule)
            .HasForeignKey(c => c.DecisionRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Outputs)
            .WithOne(o => o.DecisionRule)
            .HasForeignKey(o => o.DecisionRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.DecisionTableId, e.Priority })
            .HasDatabaseName("ix_decision_rules_table_priority");
    }
}

public class RuleConditionConfiguration : IEntityTypeConfiguration<RuleCondition>
{
    public void Configure(EntityTypeBuilder<RuleCondition> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DecisionRuleId)
            .IsRequired();

        builder.Property(e => e.ColumnKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Operator)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Value2)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}

public class RuleOutputConfiguration : IEntityTypeConfiguration<RuleOutput>
{
    public void Configure(EntityTypeBuilder<RuleOutput> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DecisionRuleId)
            .IsRequired();

        builder.Property(e => e.ColumnKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
