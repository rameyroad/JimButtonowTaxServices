using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Clients.Commands.ArchiveClient;

public class ArchiveClientHandler : IRequestHandler<ArchiveClientCommand, bool>
{
    private readonly DbContext _dbContext;

    public ArchiveClientHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(ArchiveClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Set<Client>()
            .Where(c => c.DeletedAt == null)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
        {
            return false;
        }

        client.Archive();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
