using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class DecisionTable : SoftDeletableEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public PublishStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public Guid? PublishedByUserId { get; private set; }
    public int Version { get; private set; }

    private readonly List<DecisionTableColumn> _columns = [];
    public IReadOnlyCollection<DecisionTableColumn> Columns => _columns.AsReadOnly();

    private readonly List<DecisionRule> _rules = [];
    public IReadOnlyCollection<DecisionRule> Rules => _rules.AsReadOnly();

#pragma warning disable CS8618 // Required for EF Core
    private DecisionTable() { }
#pragma warning restore CS8618

    public DecisionTable(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        Description = description?.Trim();
        Status = PublishStatus.Draft;
        Version = 1;
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

    public void AddColumn(DecisionTableColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);
        _columns.Add(column);
        SetUpdatedAt();
    }

    public void RemoveColumn(DecisionTableColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);
        _columns.Remove(column);
        SetUpdatedAt();
    }

    public void AddRule(DecisionRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules.Add(rule);
        SetUpdatedAt();
    }

    public void RemoveRule(DecisionRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules.Remove(rule);
        SetUpdatedAt();
    }

    public void Publish(Guid publishedByUserId)
    {
        Status = PublishStatus.Published;
        PublishedAt = DateTime.UtcNow;
        PublishedByUserId = publishedByUserId;
        Version++;
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
