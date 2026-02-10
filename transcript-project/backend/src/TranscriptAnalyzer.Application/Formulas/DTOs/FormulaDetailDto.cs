using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Formulas.DTOs;

public record FormulaDetailDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Expression { get; init; }
    public required string InputVariables { get; init; }
    public required DataType OutputType { get; init; }
    public required PublishStatus Status { get; init; }
    public DateTime? PublishedAt { get; init; }
    public Guid? PublishedByUserId { get; init; }
    public required int Version { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
