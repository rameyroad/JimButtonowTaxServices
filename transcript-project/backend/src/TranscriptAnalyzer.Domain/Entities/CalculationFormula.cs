using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class CalculationFormula : SoftDeletableEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Expression { get; private set; }
    public string InputVariables { get; private set; }
    public DataType OutputType { get; private set; }
    public PublishStatus Status { get; private set; }
    public int Version { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public Guid? PublishedByUserId { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private CalculationFormula() { }
#pragma warning restore CS8618

    public CalculationFormula(
        string name,
        string expression,
        string inputVariables,
        DataType outputType,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputVariables);

        Name = name.Trim();
        Expression = expression.Trim();
        InputVariables = inputVariables;
        OutputType = outputType;
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

    public void UpdateExpression(string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);
        Expression = expression.Trim();
        SetUpdatedAt();
    }

    public void UpdateInputVariables(string inputVariables)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputVariables);
        InputVariables = inputVariables;
        SetUpdatedAt();
    }

    public void UpdateOutputType(DataType outputType)
    {
        OutputType = outputType;
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
}
