using MediatR;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;

namespace TranscriptAnalyzer.Application.HumanTasks.Commands.CompleteHumanTask;

public record CompleteHumanTaskCommand : IRequest<HumanTaskDetailDto?>
{
    public Guid Id { get; init; }
    public string? Decision { get; init; }
    public string? Notes { get; init; }
}
