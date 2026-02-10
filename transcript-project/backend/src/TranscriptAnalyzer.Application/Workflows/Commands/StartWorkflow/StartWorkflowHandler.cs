using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Commands.StartWorkflow;

public class StartWorkflowHandler : IRequestHandler<StartWorkflowCommand, CaseWorkflowDetailDto>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IWorkflowEngine _workflowEngine;
    private readonly IMapper _mapper;

    public StartWorkflowHandler(
        DbContext dbContext,
        ITenantContext tenantContext,
        IWorkflowEngine workflowEngine,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _workflowEngine = workflowEngine;
        _mapper = mapper;
    }

    public async Task<CaseWorkflowDetailDto> Handle(
        StartWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        var organizationId = _tenantContext.OrganizationId
            ?? throw new UnauthorizedAccessException("Organization context is required.");
        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        // Verify the client exists and belongs to this tenant
        var clientExists = await _dbContext.Set<Client>()
            .AnyAsync(c => c.Id == request.ClientId, cancellationToken);

        if (!clientExists)
            throw new KeyNotFoundException($"Client with ID {request.ClientId} was not found.");

        // Verify the workflow definition exists and is published
        var workflowDefinition = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == request.WorkflowDefinitionId, cancellationToken);

        if (workflowDefinition is null)
            throw new KeyNotFoundException($"Workflow definition with ID {request.WorkflowDefinitionId} was not found.");

        if (workflowDefinition.Status != PublishStatus.Published)
            throw new InvalidOperationException("Cannot start a workflow that is not published.");

        // Create case workflow
        var caseWorkflow = new CaseWorkflow(
            organizationId,
            request.ClientId,
            request.WorkflowDefinitionId,
            workflowDefinition.CurrentVersion,
            userId);

        _dbContext.Set<CaseWorkflow>().Add(caseWorkflow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Start the workflow execution
        await _workflowEngine.StartWorkflowAsync(caseWorkflow, cancellationToken);

        // Reload with full navigation properties
        var result = await _dbContext.Set<CaseWorkflow>()
            .Include(cw => cw.WorkflowDefinition)
            .Include(cw => cw.StepExecutions)
                .ThenInclude(se => se.WorkflowStep)
            .FirstAsync(cw => cw.Id == caseWorkflow.Id, cancellationToken);

        return _mapper.Map<CaseWorkflowDetailDto>(result);
    }
}
