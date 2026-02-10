using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Queries.GetCaseWorkflow;

public class GetCaseWorkflowHandler : IRequestHandler<GetCaseWorkflowQuery, CaseWorkflowDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCaseWorkflowHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CaseWorkflowDetailDto?> Handle(
        GetCaseWorkflowQuery request,
        CancellationToken cancellationToken)
    {
        var caseWorkflow = await _dbContext.Set<CaseWorkflow>()
            .Include(cw => cw.WorkflowDefinition)
            .Include(cw => cw.StepExecutions)
                .ThenInclude(se => se.WorkflowStep)
            .FirstOrDefaultAsync(cw => cw.Id == request.CaseWorkflowId && cw.ClientId == request.ClientId,
                cancellationToken);

        if (caseWorkflow is null)
            return null;

        return _mapper.Map<CaseWorkflowDetailDto>(caseWorkflow);
    }
}
