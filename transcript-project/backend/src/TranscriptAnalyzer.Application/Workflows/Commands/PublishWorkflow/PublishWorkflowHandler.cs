using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.PublishWorkflow;

public class PublishWorkflowHandler : IRequestHandler<PublishWorkflowCommand, WorkflowDefinitionDetailDto?>
{
    private static readonly JsonSerializerOptions SnapshotOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public PublishWorkflowHandler(
        DbContext dbContext,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<WorkflowDefinitionDetailDto?> Handle(
        PublishWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .Include(wd => wd.Versions)
            .FirstOrDefaultAsync(wd => wd.Id == request.Id, cancellationToken);

        if (workflow is null)
            return null;

        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        // Publish the workflow (increments CurrentVersion)
        workflow.Publish(userId);

        // Deactivate any previously active versions
        foreach (var existingVersion in workflow.Versions.Where(v => v.IsActive))
        {
            existingVersion.Deactivate();
        }

        // Create a snapshot of the current workflow definition + steps
        var snapshot = new WorkflowSnapshot
        {
            Name = workflow.Name,
            Description = workflow.Description,
            Category = workflow.Category,
            Steps = workflow.Steps.OrderBy(s => s.SortOrder).Select(s => new WorkflowStepSnapshot
            {
                Id = s.Id,
                Name = s.Name,
                StepType = s.StepType.ToString(),
                SortOrder = s.SortOrder,
                Configuration = s.Configuration,
                NextStepOnSuccessId = s.NextStepOnSuccessId,
                NextStepOnFailureId = s.NextStepOnFailureId,
                IsRequired = s.IsRequired
            }).ToList()
        };

        var snapshotJson = JsonSerializer.Serialize(snapshot, SnapshotOptions);

        // Create a new version record
        var version = new WorkflowVersion(
            workflow.Id,
            workflow.CurrentVersion,
            userId,
            snapshotJson);

        _dbContext.Set<WorkflowVersion>().Add(version);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowDefinitionDetailDto>(workflow);
    }

#pragma warning disable CA1812 // Instantiated via JSON serialization
    private sealed class WorkflowSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public List<WorkflowStepSnapshot> Steps { get; set; } = [];
    }

    private sealed class WorkflowStepSnapshot
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StepType { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string? Configuration { get; set; }
        public Guid? NextStepOnSuccessId { get; set; }
        public Guid? NextStepOnFailureId { get; set; }
        public bool IsRequired { get; set; }
    }
#pragma warning restore CA1812
}
