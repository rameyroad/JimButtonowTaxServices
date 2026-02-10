using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Common.Exceptions;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Interfaces;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateTaxIdentifier;

public class UpdateTaxIdentifierHandler : IRequestHandler<UpdateTaxIdentifierCommand, ClientDto?>
{
    private readonly DbContext _dbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public UpdateTaxIdentifierHandler(
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

    public async Task<ClientDto?> Handle(UpdateTaxIdentifierCommand request, CancellationToken cancellationToken)
    {
        var organizationId = _tenantContext.OrganizationId
            ?? throw new UnauthorizedAccessException("Organization context is required.");

        var client = await _dbContext.Set<Client>()
            .Where(c => c.DeletedAt == null)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return null;
        }

        // Check for optimistic concurrency conflict
        if (client.Version != request.Version)
        {
            throw new ConcurrencyConflictException(
                $"The client has been modified by another user. Current version: {client.Version}, your version: {request.Version}");
        }

        // Normalize tax identifier (remove dashes)
        var normalizedTaxId = request.TaxIdentifier.Replace("-", "", StringComparison.Ordinal);
        var last4 = normalizedTaxId[^4..];

        // Check for duplicate within tenant, excluding the current client
        await CheckForDuplicateTaxIdentifierAsync(organizationId, normalizedTaxId, last4, request.Id, cancellationToken);

        // Encrypt tax identifier
        var encryptedTaxId = _encryptionService.Encrypt(normalizedTaxId);

        // Update tax identifier on entity
        client.UpdateTaxIdentifier(encryptedTaxId, last4);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ClientDto>(client);
    }

    private async Task CheckForDuplicateTaxIdentifierAsync(
        Guid organizationId,
        string normalizedTaxId,
        string last4,
        Guid excludeClientId,
        CancellationToken cancellationToken)
    {
        // Query potential duplicates by last4 (indexed for performance), excluding current client
        var potentialDuplicates = await _dbContext.Set<Client>()
            .Where(c => c.OrganizationId == organizationId && c.TaxIdentifierLast4 == last4 && c.Id != excludeClientId)
            .Select(c => new { c.Id, c.TaxIdentifier })
            .ToListAsync(cancellationToken);

        // Decrypt and compare each potential match
        foreach (var candidate in potentialDuplicates)
        {
            var decryptedTaxId = _encryptionService.Decrypt(candidate.TaxIdentifier);
            if (string.Equals(decryptedTaxId, normalizedTaxId, StringComparison.Ordinal))
            {
                throw new DuplicateTaxIdentifierException(
                    "A client with this tax identifier already exists.");
            }
        }
    }
}
