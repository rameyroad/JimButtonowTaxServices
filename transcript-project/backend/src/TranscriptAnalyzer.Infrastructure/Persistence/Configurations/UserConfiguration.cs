using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("IX_Users_OrganizationId");

        builder.Property(e => e.Auth0UserId)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(e => e.Auth0UserId)
            .IsUnique()
            .HasDatabaseName("IX_Users_Auth0UserId")
            .HasFilter("[Auth0UserId] <> ''");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(e => new { e.OrganizationId, e.Email })
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_OrganizationId");

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(UserStatus.Active);

        builder.Property(e => e.InvitedAt);

        builder.Property(e => e.InvitedByUserId);

        builder.Property(e => e.LastLoginAt);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InvitedBy)
            .WithMany()
            .HasForeignKey(e => e.InvitedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(e => e.FullName);
    }
}
