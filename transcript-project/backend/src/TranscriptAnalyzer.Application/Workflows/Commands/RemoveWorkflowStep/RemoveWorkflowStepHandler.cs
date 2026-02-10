using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.RemoveWorkflowStep;

public class RemoveWorkflowStepHandler : IRequestHandler<RemoveWorkflowStepCommand, bool>
{
    private readonly DbContext _dbContext;

    public RemoveWorkflowStepHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        RemoveWorkflowStepCommand request,
        CancellationToken cancellationToken)
    {
        var step = await _dbContext.Set<WorkflowStep>()
            .FirstOrDefaultAsync(s => s.Id == request.StepId && s.WorkflowDefinitionId == request.WorkflowDefinitionId,
                cancellationToken);

        if (step is null)
            return false;

        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == request.WorkflowDefinitionId, cancellationToken);

        if (workflow is null)
            return false;

        workflow.RemoveStep(step);
        _dbContext.Set<WorkflowStep>().Remove(step);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
