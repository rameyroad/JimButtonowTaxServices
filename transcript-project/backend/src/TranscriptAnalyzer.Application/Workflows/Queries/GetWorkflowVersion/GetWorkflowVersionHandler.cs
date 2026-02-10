using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Queries.GetWorkflowVersion;

public class GetWorkflowVersionHandler : IRequestHandler<GetWorkflowVersionQuery, WorkflowVersionDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetWorkflowVersionHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowVersionDetailDto?> Handle(
        GetWorkflowVersionQuery request,
        CancellationToken cancellationToken)
    {
        var version = await _dbContext.Set<WorkflowVersion>()
            .FirstOrDefaultAsync(v =>
                v.Id == request.VersionId &&
                v.WorkflowDefinitionId == request.WorkflowDefinitionId,
                cancellationToken);

        if (version is null)
            return null;

        return _mapper.Map<WorkflowVersionDetailDto>(version);
    }
}
