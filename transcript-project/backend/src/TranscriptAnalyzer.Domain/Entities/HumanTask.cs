using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class HumanTask : TenantEntity
{
    public Guid CaseWorkflowId { get; private set; }
    public Guid StepExecutionId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public HumanTaskStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedByUserId { get; private set; }
    public string? Decision { get; private set; }
    public string? Notes { get; private set; }

    public CaseWorkflow CaseWorkflow { get; private set; } = null!;
    public StepExecution StepExecution { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private HumanTask() { }
#pragma warning restore CS8618

    public HumanTask(
        Guid organizationId,
        Guid caseWorkflowId,
        Guid stepExecutionId,
        string title,
        string? description = null,
        Guid? assignedToUserId = null,
        DateTime? dueDate = null) : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        CaseWorkflowId = caseWorkflowId;
        StepExecutionId = stepExecutionId;
        Title = title.Trim();
        Description = description?.Trim();
        AssignedToUserId = assignedToUserId;
        DueDate = dueDate;
        Status = HumanTaskStatus.Pending;
    }

    public void StartWork()
    {
        Status = HumanTaskStatus.InProgress;
        SetUpdatedAt();
    }

    public void Complete(Guid completedByUserId, string? decision = null, string? notes = null)
    {
        Status = HumanTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = completedByUserId;
        Decision = decision?.Trim();
        Notes = notes?.Trim();
        SetUpdatedAt();
    }

    public void Reassign(Guid newAssigneeUserId)
    {
        AssignedToUserId = newAssigneeUserId;
        Status = HumanTaskStatus.Reassigned;
        SetUpdatedAt();
    }

    public void Escalate()
    {
        Status = HumanTaskStatus.Escalated;
        SetUpdatedAt();
    }
}
