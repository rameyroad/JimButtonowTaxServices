using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common.Exceptions;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateClient;

public class UpdateClientHandler : IRequestHandler<UpdateClientCommand, ClientDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateClientHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientDto?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Set<Client>()
            .Include(c => c.Address)
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

        // Track if any changes were made
        var hasChanges = false;

        // Update individual-specific fields
        if (client.ClientType == ClientType.Individual)
        {
            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
            {
                var firstName = request.FirstName ?? client.FirstName ?? string.Empty;
                var lastName = request.LastName ?? client.LastName ?? string.Empty;
                client.UpdateIndividualName(firstName, lastName);
                hasChanges = true;
            }
        }

        // Update business-specific fields
        if (client.ClientType == ClientType.Business)
        {
            if (!string.IsNullOrEmpty(request.BusinessName) || request.EntityType.HasValue)
            {
                var businessName = request.BusinessName ?? client.BusinessName ?? string.Empty;
                var entityType = request.EntityType ?? client.EntityType ?? BusinessEntityType.LLC;
                var responsibleParty = request.ResponsibleParty ?? client.ResponsibleParty;
                client.UpdateBusinessInfo(businessName, entityType, responsibleParty);
                hasChanges = true;
            }
            else if (request.ResponsibleParty != null)
            {
                // Only updating responsible party
                client.UpdateBusinessInfo(
                    client.BusinessName ?? string.Empty,
                    client.EntityType ?? BusinessEntityType.LLC,
                    request.ResponsibleParty);
                hasChanges = true;
            }
        }

        // Update contact info
        if (!string.IsNullOrEmpty(request.Email) || request.Phone != null)
        {
            var email = request.Email ?? client.Email;
            var phone = request.Phone ?? client.Phone;
            client.UpdateContactInfo(email, phone);
            hasChanges = true;
        }

        // Update address
        if (request.Address != null)
        {
            var address = new Address(
                request.Address.Street1,
                request.Address.Street2,
                request.Address.City,
                request.Address.State,
                request.Address.PostalCode,
                request.Address.Country);
            client.UpdateAddress(address);
            hasChanges = true;
        }

        // Update notes
        if (request.Notes != null)
        {
            client.UpdateNotes(request.Notes);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return _mapper.Map<ClientDto>(client);
    }
}
