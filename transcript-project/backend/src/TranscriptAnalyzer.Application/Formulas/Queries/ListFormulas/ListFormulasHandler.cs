using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Queries.ListFormulas;

public class ListFormulasHandler : IRequestHandler<ListFormulasQuery, PaginatedList<FormulaListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListFormulasHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<FormulaListItemDto>> Handle(
        ListFormulasQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<CalculationFormula>().AsQueryable();

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(f => f.Status == request.Status.Value);
        }

        // Search by name or description
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim()}%";
#pragma warning disable CA1307, CA1304
            query = query.Where(f =>
                EF.Functions.Like(f.Name, searchPattern) ||
                EF.Functions.Like(f.Description ?? string.Empty, searchPattern));
#pragma warning restore CA1307, CA1304
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(f => new FormulaListItemDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            OutputType = f.OutputType,
            Status = f.Status,
            PublishedAt = f.PublishedAt,
            Version = f.Version,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });

        return await PaginatedList<FormulaListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<CalculationFormula> ApplySorting(
        IQueryable<CalculationFormula> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "NAME" => isDescending
                ? query.OrderByDescending(f => f.Name)
                : query.OrderBy(f => f.Name),
            "STATUS" => isDescending
                ? query.OrderByDescending(f => f.Status)
                : query.OrderBy(f => f.Status),
            "OUTPUTTYPE" => isDescending
                ? query.OrderByDescending(f => f.OutputType)
                : query.OrderBy(f => f.OutputType),
            "CREATEDAT" => isDescending
                ? query.OrderByDescending(f => f.CreatedAt)
                : query.OrderBy(f => f.CreatedAt),
            "UPDATEDAT" => isDescending
                ? query.OrderByDescending(f => f.UpdatedAt)
                : query.OrderBy(f => f.UpdatedAt),
            _ => query.OrderBy(f => f.Name)
        };
    }
}
