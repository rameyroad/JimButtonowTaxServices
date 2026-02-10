using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class WorkflowStep : BaseEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public string Name { get; private set; }
    public WorkflowStepType StepType { get; private set; }
    public int SortOrder { get; private set; }
    public string? Configuration { get; private set; }
    public Guid? NextStepOnSuccessId { get; private set; }
    public Guid? NextStepOnFailureId { get; private set; }
    public bool IsRequired { get; private set; }

    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;
    public WorkflowStep? NextStepOnSuccess { get; private set; }
    public WorkflowStep? NextStepOnFailure { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private WorkflowStep() { }
#pragma warning restore CS8618

    public WorkflowStep(
        Guid workflowDefinitionId,
        string name,
        WorkflowStepType stepType,
        int sortOrder,
        string? configuration = null,
        bool isRequired = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        WorkflowDefinitionId = workflowDefinitionId;
        Name = name.Trim();
        StepType = stepType;
        SortOrder = sortOrder;
        Configuration = configuration;
        IsRequired = isRequired;
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdatedAt();
    }

    public void UpdateStepType(WorkflowStepType stepType)
    {
        StepType = stepType;
        SetUpdatedAt();
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        SetUpdatedAt();
    }

    public void UpdateConfiguration(string? configuration)
    {
        Configuration = configuration;
        SetUpdatedAt();
    }

    public void SetNextStepOnSuccess(Guid? stepId)
    {
        NextStepOnSuccessId = stepId;
        SetUpdatedAt();
    }

    public void SetNextStepOnFailure(Guid? stepId)
    {
        NextStepOnFailureId = stepId;
        SetUpdatedAt();
    }

    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
        SetUpdatedAt();
    }
}
