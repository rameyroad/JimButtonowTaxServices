using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class StepExecution : TenantEntity
{
    public Guid CaseWorkflowId { get; private set; }
    public Guid WorkflowStepId { get; private set; }
    public StepExecutionStatus Status { get; private set; }
    public string? InputData { get; private set; }
    public string? OutputData { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    public CaseWorkflow CaseWorkflow { get; private set; } = null!;
    public WorkflowStep WorkflowStep { get; private set; } = null!;

    private StepExecution() { }

    public StepExecution(
        Guid organizationId,
        Guid caseWorkflowId,
        Guid workflowStepId) : base(organizationId)
    {
        CaseWorkflowId = caseWorkflowId;
        WorkflowStepId = workflowStepId;
        Status = StepExecutionStatus.Pending;
    }

    public void Start(string? inputData = null)
    {
        Status = StepExecutionStatus.Running;
        StartedAt = DateTime.UtcNow;
        InputData = inputData;
        SetUpdatedAt();
    }

    public void Complete(string? outputData = null)
    {
        Status = StepExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        OutputData = outputData;
        SetUpdatedAt();
    }

    public void Fail(string errorMessage)
    {
        Status = StepExecutionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        SetUpdatedAt();
    }

    public void Skip()
    {
        Status = StepExecutionStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
