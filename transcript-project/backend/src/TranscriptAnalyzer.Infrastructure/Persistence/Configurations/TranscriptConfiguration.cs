using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Infrastructure.Persistence.Configurations;

public class TranscriptConfiguration : IEntityTypeConfiguration<Transcript>
{
    public void Configure(EntityTypeBuilder<Transcript> builder)
    {
        builder.ToTable("Transcripts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("IX_Transcripts_OrganizationId");

        builder.Property(e => e.ClientId)
            .IsRequired();

        builder.HasIndex(e => e.ClientId)
            .HasDatabaseName("IX_Transcripts_ClientId");

        builder.Property(e => e.AuthorizationId)
            .IsRequired();

        builder.HasIndex(e => e.AuthorizationId)
            .HasDatabaseName("IX_Transcripts_AuthorizationId");

        builder.Property(e => e.UploadedByUserId)
            .IsRequired();

        builder.Property(e => e.TranscriptType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.TaxYear)
            .IsRequired();

        builder.HasIndex(e => new { e.OrganizationId, e.TaxYear })
            .HasDatabaseName("IX_Transcripts_TaxYear");

        builder.Property(e => e.BlobPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.UploadedAt)
            .IsRequired();

        builder.Property(e => e.LastAccessedAt);

        builder.Property(e => e.LastAccessedByUserId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Client)
            .WithMany(c => c.Transcripts)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Authorization)
            .WithMany(a => a.Transcripts)
            .HasForeignKey(e => e.AuthorizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UploadedBy)
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.LastAccessedBy)
            .WithMany()
            .HasForeignKey(e => e.LastAccessedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
