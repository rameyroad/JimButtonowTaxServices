using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => new { e.OrganizationId, e.Timestamp })
            .IsDescending(false, true)
            .HasDatabaseName("ix_audit_logs_organization_id_timestamp");

        builder.Property(e => e.UserId);

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("ix_audit_logs_user_id");

        builder.Property(e => e.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(e => new { e.Action, e.Timestamp })
            .IsDescending(false, true)
            .HasDatabaseName("ix_audit_logs_action");

        builder.Property(e => e.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.EntityId)
            .IsRequired();

        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("ix_audit_logs_entity_type_entity_id");

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
