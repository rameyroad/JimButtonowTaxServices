using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowStep;

public class UpdateWorkflowStepHandler : IRequestHandler<UpdateWorkflowStepCommand, WorkflowStepDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateWorkflowStepHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowStepDto?> Handle(
        UpdateWorkflowStepCommand request,
        CancellationToken cancellationToken)
    {
        var step = await _dbContext.Set<WorkflowStep>()
            .FirstOrDefaultAsync(s => s.Id == request.StepId && s.WorkflowDefinitionId == request.WorkflowDefinitionId,
                cancellationToken);

        if (step is null)
            return null;

        if (request.Name is not null)
            step.UpdateName(request.Name);

        if (request.StepType.HasValue)
            step.UpdateStepType(request.StepType.Value);

        if (request.SortOrder.HasValue)
            step.UpdateSortOrder(request.SortOrder.Value);

        if (request.Configuration is not null)
            step.UpdateConfiguration(request.Configuration);

        if (request.NextStepOnSuccessId.HasValue)
            step.SetNextStepOnSuccess(request.NextStepOnSuccessId);

        if (request.NextStepOnFailureId.HasValue)
            step.SetNextStepOnFailure(request.NextStepOnFailureId);

        if (request.IsRequired.HasValue)
            step.SetRequired(request.IsRequired.Value);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowStepDto>(step);
    }
}
