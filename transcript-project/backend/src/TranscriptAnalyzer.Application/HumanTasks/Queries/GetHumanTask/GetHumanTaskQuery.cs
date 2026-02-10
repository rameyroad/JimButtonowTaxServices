using MediatR;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;

namespace TranscriptAnalyzer.Application.HumanTasks.Queries.GetHumanTask;

public record GetHumanTaskQuery(Guid Id) : IRequest<HumanTaskDetailDto?>;
