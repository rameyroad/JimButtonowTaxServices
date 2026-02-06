using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => new { e.OrganizationId, e.Timestamp })
            .IsDescending(false, true)
            .HasDatabaseName("IX_AuditLogs_OrganizationId_Timestamp");

        builder.Property(e => e.UserId);

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.Property(e => e.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(e => new { e.Action, e.Timestamp })
            .IsDescending(false, true)
            .HasDatabaseName("IX_AuditLogs_Action");

        builder.Property(e => e.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.EntityId)
            .IsRequired();

        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("IX_AuditLogs_EntityType_EntityId");

        builder.Property(e => e.IpAddress)
            .HasMaxLength(45);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.BeforeState);

        builder.Property(e => e.AfterState);

        builder.Property(e => e.Metadata);

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
