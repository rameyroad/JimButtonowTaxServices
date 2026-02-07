using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Common.Exceptions;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.Interfaces;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Clients.Commands.CreateClient;

public class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly DbContext _dbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public CreateClientHandler(
        DbContext dbContext,
        IEncryptionService encryptionService,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _encryptionService = encryptionService;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _tenantContext.OrganizationId
            ?? throw new UnauthorizedAccessException("Organization context is required.");
        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        // Normalize tax identifier (remove dashes)
        var normalizedTaxId = NormalizeTaxIdentifier(request.TaxIdentifier);
        var last4 = normalizedTaxId[^4..];

        // Check for duplicate within tenant
        await CheckForDuplicateTaxIdentifierAsync(organizationId, normalizedTaxId, last4, cancellationToken);

        // Encrypt tax identifier
        var encryptedTaxId = _encryptionService.Encrypt(normalizedTaxId);

        // Create address value object
        var address = new Address(
            request.Address.Street1,
            request.Address.Street2,
            request.Address.City,
            request.Address.State,
            request.Address.PostalCode,
            request.Address.Country);

        // Create client entity
        Client client;
        if (request.ClientType == ClientType.Individual)
        {
            client = Client.CreateIndividual(
                organizationId,
                request.FirstName!,
                request.LastName!,
                encryptedTaxId,
                last4,
                request.Email,
                address,
                userId,
                request.Phone,
                request.Notes);
        }
        else
        {
            client = Client.CreateBusiness(
                organizationId,
                request.BusinessName!,
                request.EntityType!.Value,
                encryptedTaxId,
                last4,
                request.Email,
                address,
                userId,
                request.ResponsibleParty,
                request.Phone,
                request.Notes);
        }

        _dbContext.Set<Client>().Add(client);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ClientDto>(client);
    }

    private async Task CheckForDuplicateTaxIdentifierAsync(
        Guid organizationId,
        string normalizedTaxId,
        string last4,
        CancellationToken cancellationToken)
    {
        // Query potential duplicates by last4 (indexed for performance)
        var potentialDuplicates = await _dbContext.Set<Client>()
            .Where(c => c.OrganizationId == organizationId && c.TaxIdentifierLast4 == last4)
            .Select(c => new { c.Id, c.TaxIdentifier })
            .ToListAsync(cancellationToken);

        // Decrypt and compare each potential match
        foreach (var candidate in potentialDuplicates)
        {
            var decryptedTaxId = _encryptionService.Decrypt(candidate.TaxIdentifier);
            if (string.Equals(decryptedTaxId, normalizedTaxId, StringComparison.Ordinal))
            {
                throw new DuplicateTaxIdentifierException(
                    $"A client with this tax identifier already exists.");
            }
        }
    }

    private static string NormalizeTaxIdentifier(string taxIdentifier)
    {
        // Remove dashes for consistent storage and comparison
        return taxIdentifier.Replace("-", "", StringComparison.Ordinal);
    }
}
