using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.HasIndex(e => new { e.UserId, e.ReadAt })
            .HasDatabaseName("ix_notifications_user_id_read_at");

        builder.HasIndex(e => new { e.OrganizationId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("ix_notifications_created_at");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50);

        builder.Property(e => e.EntityId);

        builder.Property<List<NotificationChannel>>("_channels")
            .HasColumnName("channels")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<NotificationChannel>>(v, (JsonSerializerOptions?)null) ?? new List<NotificationChannel>());

        builder.Property(e => e.EmailSentAt);

        builder.Property(e => e.ReadAt);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.Channels);
        builder.Ignore(e => e.IsRead);
    }
}
