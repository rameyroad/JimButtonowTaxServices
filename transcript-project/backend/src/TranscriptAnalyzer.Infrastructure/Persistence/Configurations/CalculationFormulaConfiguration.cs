using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class CalculationFormulaConfiguration : IEntityTypeConfiguration<CalculationFormula>
{
    public void Configure(EntityTypeBuilder<CalculationFormula> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Expression)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.InputVariables)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.OutputType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PublishStatus.Draft);

        builder.Property(e => e.Version)
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

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("ix_calculation_formulas_name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_calculation_formulas_status");
    }
}
