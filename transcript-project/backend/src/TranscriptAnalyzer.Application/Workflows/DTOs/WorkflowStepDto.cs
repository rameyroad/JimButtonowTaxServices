using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record WorkflowStepDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required WorkflowStepType StepType { get; init; }
    public required int SortOrder { get; init; }
    public string? Configuration { get; init; }
    public Guid? NextStepOnSuccessId { get; init; }
    public Guid? NextStepOnFailureId { get; init; }
    public required bool IsRequired { get; init; }
}
