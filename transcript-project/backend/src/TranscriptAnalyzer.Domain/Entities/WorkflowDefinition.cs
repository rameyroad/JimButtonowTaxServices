using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class WorkflowDefinition : SoftDeletableEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Category { get; private set; }
    public PublishStatus Status { get; private set; }
    public int CurrentVersion { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public Guid? PublishedByUserId { get; private set; }

    private readonly List<WorkflowStep> _steps = [];
    public IReadOnlyCollection<WorkflowStep> Steps => _steps.AsReadOnly();

#pragma warning disable CS8618 // Required for EF Core
    private WorkflowDefinition() { }
#pragma warning restore CS8618

    public WorkflowDefinition(string name, string? description = null, string? category = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        Description = description?.Trim();
        Category = category?.Trim();
        Status = PublishStatus.Draft;
        CurrentVersion = 1;
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdatedAt();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        SetUpdatedAt();
    }

    public void UpdateCategory(string? category)
    {
        Category = category?.Trim();
        SetUpdatedAt();
    }

    public void AddStep(WorkflowStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Add(step);
        SetUpdatedAt();
    }

    public void RemoveStep(WorkflowStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Remove(step);
        SetUpdatedAt();
    }

    public void Publish(Guid publishedByUserId)
    {
        if (_steps.Count == 0)
            throw new InvalidOperationException("Cannot publish a workflow with no steps.");

        Status = PublishStatus.Published;
        PublishedAt = DateTime.UtcNow;
        PublishedByUserId = publishedByUserId;
        CurrentVersion++;
        SetUpdatedAt();
    }

    public void Unpublish()
    {
        Status = PublishStatus.Draft;
        SetUpdatedAt();
    }

    public void Archive()
    {
        Status = PublishStatus.Archived;
        SetUpdatedAt();
    }
}
