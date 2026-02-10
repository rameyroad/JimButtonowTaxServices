using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Formulas.Queries.ListFormulas;

public record ListFormulasQuery : IRequest<PaginatedList<FormulaListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public PublishStatus? Status { get; init; }
    public string SortBy { get; init; } = "name";
    public string SortOrder { get; init; } = "asc";
}
