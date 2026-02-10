using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class DecisionTableColumn : BaseEntity
{
    public Guid DecisionTableId { get; private set; }
    public string Name { get; private set; }
    public string Key { get; private set; }
    public DataType DataType { get; private set; }
    public bool IsInput { get; private set; }
    public int SortOrder { get; private set; }

    public DecisionTable DecisionTable { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private DecisionTableColumn() { }
#pragma warning restore CS8618

    public DecisionTableColumn(
        Guid decisionTableId,
        string name,
        string key,
        DataType dataType,
        bool isInput,
        int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        DecisionTableId = decisionTableId;
        Name = name.Trim();
        Key = key.Trim();
        DataType = dataType;
        IsInput = isInput;
        SortOrder = sortOrder;
    }

    public void Update(string name, string key, DataType dataType, int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        Name = name.Trim();
        Key = key.Trim();
        DataType = dataType;
        SortOrder = sortOrder;
        SetUpdatedAt();
    }
}
