using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class CaseWorkflow : TenantEntity
{
    public Guid ClientId { get; private set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public int WorkflowVersion { get; private set; }
    public Guid? WorkflowVersionId { get; private set; }
    public WorkflowExecutionStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid StartedByUserId { get; private set; }
    public Guid? CurrentStepId { get; private set; }
    public string? ErrorMessage { get; private set; }

    public Client Client { get; private set; } = null!;
    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;
    public WorkflowVersion? WorkflowVersionEntity { get; private set; }
    public WorkflowStep? CurrentStep { get; private set; }

    private readonly List<StepExecution> _stepExecutions = [];
    public IReadOnlyCollection<StepExecution> StepExecutions => _stepExecutions.AsReadOnly();

    private CaseWorkflow() { }

    public CaseWorkflow(
        Guid organizationId,
        Guid clientId,
        Guid workflowDefinitionId,
        int workflowVersion,
        Guid startedByUserId) : base(organizationId)
    {
        ClientId = clientId;
        WorkflowDefinitionId = workflowDefinitionId;
        WorkflowVersion = workflowVersion;
        StartedByUserId = startedByUserId;
        Status = WorkflowExecutionStatus.NotStarted;
    }

    public void SetWorkflowVersionId(Guid workflowVersionId)
    {
        WorkflowVersionId = workflowVersionId;
    }

    public void Start(Guid? firstStepId)
    {
        Status = WorkflowExecutionStatus.Running;
        StartedAt = DateTime.UtcNow;
        CurrentStepId = firstStepId;
        SetUpdatedAt();
    }

    public void MoveToStep(Guid? stepId)
    {
        CurrentStepId = stepId;
        SetUpdatedAt();
    }

    public void Pause()
    {
        Status = WorkflowExecutionStatus.Paused;
        SetUpdatedAt();
    }

    public void Resume(Guid? nextStepId)
    {
        Status = WorkflowExecutionStatus.Running;
        CurrentStepId = nextStepId;
        SetUpdatedAt();
    }

    public void Complete()
    {
        Status = WorkflowExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CurrentStepId = null;
        SetUpdatedAt();
    }

    public void Fail(string errorMessage)
    {
        Status = WorkflowExecutionStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Cancel(string? reason = null)
    {
        Status = WorkflowExecutionStatus.Cancelled;
        ErrorMessage = reason;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void AddStepExecution(StepExecution stepExecution)
    {
        ArgumentNullException.ThrowIfNull(stepExecution);
        _stepExecutions.Add(stepExecution);
    }
}
