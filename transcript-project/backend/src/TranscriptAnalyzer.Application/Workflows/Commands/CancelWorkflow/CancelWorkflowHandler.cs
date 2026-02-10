using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Commands.CancelWorkflow;

public class CancelWorkflowHandler : IRequestHandler<CancelWorkflowCommand, CaseWorkflowDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public CancelWorkflowHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CaseWorkflowDetailDto?> Handle(
        CancelWorkflowCommand request,
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

        if (caseWorkflow.Status == WorkflowExecutionStatus.Completed ||
            caseWorkflow.Status == WorkflowExecutionStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Cannot cancel a workflow that is already {caseWorkflow.Status}.");
        }

        caseWorkflow.Cancel(request.Reason);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CaseWorkflowDetailDto>(caseWorkflow);
    }
}
