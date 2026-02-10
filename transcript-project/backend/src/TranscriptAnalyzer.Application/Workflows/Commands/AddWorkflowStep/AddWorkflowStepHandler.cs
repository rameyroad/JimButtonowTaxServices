using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.AddWorkflowStep;

public class AddWorkflowStepHandler : IRequestHandler<AddWorkflowStepCommand, WorkflowStepDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public AddWorkflowStepHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowStepDto?> Handle(
        AddWorkflowStepCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .FirstOrDefaultAsync(wd => wd.Id == request.WorkflowDefinitionId, cancellationToken);

        if (workflow is null)
            return null;

        var step = new WorkflowStep(
            request.WorkflowDefinitionId,
            request.Name,
            request.StepType,
            request.SortOrder,
            request.Configuration,
            request.IsRequired);

        if (request.NextStepOnSuccessId.HasValue)
            step.SetNextStepOnSuccess(request.NextStepOnSuccessId);

        if (request.NextStepOnFailureId.HasValue)
            step.SetNextStepOnFailure(request.NextStepOnFailureId);

        workflow.AddStep(step);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowStepDto>(step);
    }
}
