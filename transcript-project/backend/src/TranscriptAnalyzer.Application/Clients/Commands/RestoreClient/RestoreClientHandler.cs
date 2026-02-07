using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Clients.Commands.RestoreClient;

public class RestoreClientHandler : IRequestHandler<RestoreClientCommand, RestoreClientResult>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public RestoreClientHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<RestoreClientResult> Handle(RestoreClientCommand request, CancellationToken cancellationToken)
    {
        // Need to include archived clients, so don't filter by DeletedAt
        var client = await _dbContext.Set<Client>()
            .IgnoreQueryFilters()
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return new RestoreClientResult(Found: false, WasArchived: false, Client: null);
        }

        // Check if client was archived
        if (client.DeletedAt is null)
        {
            return new RestoreClientResult(Found: true, WasArchived: false, Client: null);
        }

        client.Unarchive();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ClientDto>(client);
        return new RestoreClientResult(Found: true, WasArchived: true, Client: dto);
    }
}
