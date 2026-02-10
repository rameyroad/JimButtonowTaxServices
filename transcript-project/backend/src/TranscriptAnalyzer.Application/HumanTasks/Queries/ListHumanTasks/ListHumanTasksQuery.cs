using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.HumanTasks.Queries.ListHumanTasks;

public record ListHumanTasksQuery : IRequest<PaginatedList<HumanTaskListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public HumanTaskStatus? Status { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public string SortBy { get; init; } = "createdAt";
    public string SortOrder { get; init; } = "desc";
}
