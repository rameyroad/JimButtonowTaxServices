using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Registration.Commands.RegisterOrganization;

public partial class RegisterOrganizationHandler : IRequestHandler<RegisterOrganizationCommand, RegisterOrganizationResult>
{
    private readonly DbContext _dbContext;

    public RegisterOrganizationHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RegisterOrganizationResult> Handle(
        RegisterOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // Generate slug from organization name
        var slug = GenerateSlug(request.OrganizationName);

        // Check for duplicate slug
        var slugExists = await _dbContext.Set<Organization>()
            .AnyAsync(o => o.Slug == slug, cancellationToken);

        if (slugExists)
        {
            slug = $"{slug}-{DateTime.UtcNow.Ticks % 10000}";
        }

        // Create organization
        var address = new Address(
            request.Street1,
            request.Street2,
            request.City,
            request.State,
            request.PostalCode);

        var organization = new Organization(
            request.OrganizationName,
            slug,
            request.ContactEmail,
            address);

        _dbContext.Set<Organization>().Add(organization);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Create admin user
        var adminUser = new User(
            organization.Id,
            $"dev|{Guid.NewGuid()}", // placeholder Auth0 ID
            request.AdminEmail,
            request.AdminFirstName,
            request.AdminLastName,
            UserRole.Admin);

        _dbContext.Set<User>().Add(adminUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisterOrganizationResult
        {
            OrganizationId = organization.Id,
            UserId = adminUser.Id,
            OrganizationName = organization.Name
        };
    }

#pragma warning disable CA1308 // Slugs are intentionally lowercase
    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant().Trim();
        slug = SlugRegex().Replace(slug, "-");
        slug = DashCleanupRegex().Replace(slug, "-");
        return slug.Trim('-');
    }
#pragma warning restore CA1308

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex SlugRegex();

    [GeneratedRegex("-{2,}")]
    private static partial Regex DashCleanupRegex();
}
