using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.ClientApprovals.DTOs;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.ClientApprovals.Commands.RespondToApproval;

public class RespondToApprovalHandler : IRequestHandler<RespondToApprovalCommand, ClientApprovalDto?>
{
    private readonly DbContext _dbContext;
    private readonly IWorkflowEngine _workflowEngine;

    public RespondToApprovalHandler(DbContext dbContext, IWorkflowEngine workflowEngine)
    {
        _dbContext = dbContext;
        _workflowEngine = workflowEngine;
    }

    public async Task<ClientApprovalDto?> Handle(
        RespondToApprovalCommand request,
        CancellationToken cancellationToken)
    {
        // Bypass tenant filter — public endpoint
        var approval = await _dbContext.Set<ClientApproval>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Token == request.Token && a.DeletedAt == null, cancellationToken);

        if (approval is null)
            return null;

        if (!approval.CanRespond)
        {
            return new ClientApprovalDto
            {
                Id = approval.Id,
                Title = approval.Title,
                Description = approval.Description,
                Status = approval.Status,
                TokenExpiresAt = approval.TokenExpiresAt,
                RespondedAt = approval.RespondedAt,
                ResponseNotes = approval.ResponseNotes,
                CreatedAt = approval.CreatedAt
            };
        }

        // Record the response
        if (request.Approved)
        {
            approval.Approve(request.Notes);
        }
        else
        {
            approval.Decline(request.Notes);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Resume workflow if approved
        if (request.Approved)
        {
            var caseWorkflow = await _dbContext.Set<CaseWorkflow>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(cw => cw.Id == approval.CaseWorkflowId && cw.DeletedAt == null, cancellationToken);

            if (caseWorkflow is not null && caseWorkflow.Status == WorkflowExecutionStatus.Paused)
            {
                var definition = await _dbContext.Set<WorkflowDefinition>()
                    .Include(wd => wd.Steps)
                    .FirstOrDefaultAsync(wd => wd.Id == caseWorkflow.WorkflowDefinitionId, cancellationToken);

                if (definition is not null && caseWorkflow.CurrentStepId.HasValue)
                {
                    var orderedSteps = definition.Steps.OrderBy(s => s.SortOrder).ToList();
                    var currentStep = orderedSteps.FirstOrDefault(s => s.Id == caseWorkflow.CurrentStepId.Value);
                    var currentIndex = orderedSteps.IndexOf(currentStep!);

                    Guid? nextStepId = null;
                    if (currentStep?.NextStepOnSuccessId.HasValue == true)
                    {
                        nextStepId = currentStep.NextStepOnSuccessId;
                    }
                    else if (currentIndex + 1 < orderedSteps.Count)
                    {
                        nextStepId = orderedSteps[currentIndex + 1].Id;
                    }

                    caseWorkflow.MoveToStep(nextStepId);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    await _workflowEngine.ResumeWorkflowAsync(caseWorkflow, cancellationToken);
                }
            }
        }

        return new ClientApprovalDto
        {
            Id = approval.Id,
            Title = approval.Title,
            Description = approval.Description,
            Status = approval.Status,
            TokenExpiresAt = approval.TokenExpiresAt,
            RespondedAt = approval.RespondedAt,
            ResponseNotes = approval.ResponseNotes,
            CreatedAt = approval.CreatedAt
        };
    }
}
