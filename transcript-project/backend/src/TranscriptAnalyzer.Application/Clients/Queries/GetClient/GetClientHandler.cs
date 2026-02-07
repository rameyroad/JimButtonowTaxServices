using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;

namespace TranscriptAnalyzer.Application.Clients.Queries.GetClient;

public class GetClientHandler : IRequestHandler<GetClientQuery, ClientDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetClientHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientDetailDto?> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Set<Domain.Entities.Client>()
            .Include(c => c.Address)
            .Include(c => c.CreatedBy)
            .Include(c => c.Authorizations)
            .Include(c => c.Transcripts)
            .Where(c => c.DeletedAt == null)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return null;
        }

        return _mapper.Map<ClientDetailDto>(client);
    }
}
