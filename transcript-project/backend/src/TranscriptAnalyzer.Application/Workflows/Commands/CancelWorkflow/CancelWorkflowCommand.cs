using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.CancelWorkflow;

public record CancelWorkflowCommand : IRequest<CaseWorkflowDetailDto?>
{
    public required Guid ClientId { get; init; }
    public required Guid CaseWorkflowId { get; init; }
    public string? Reason { get; init; }
}
