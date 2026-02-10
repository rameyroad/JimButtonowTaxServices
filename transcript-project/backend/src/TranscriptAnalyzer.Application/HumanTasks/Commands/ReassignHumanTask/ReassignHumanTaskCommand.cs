using MediatR;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;

namespace TranscriptAnalyzer.Application.HumanTasks.Commands.ReassignHumanTask;

public record ReassignHumanTaskCommand : IRequest<HumanTaskDetailDto?>
{
    public Guid Id { get; init; }
    public Guid AssignedToUserId { get; init; }
}
