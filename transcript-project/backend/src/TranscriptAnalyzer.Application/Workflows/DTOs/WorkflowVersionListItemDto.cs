namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record WorkflowVersionListItemDto
{
    public required Guid Id { get; init; }
    public required Guid WorkflowDefinitionId { get; init; }
    public required int VersionNumber { get; init; }
    public required DateTime PublishedAt { get; init; }
    public Guid PublishedByUserId { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
}
