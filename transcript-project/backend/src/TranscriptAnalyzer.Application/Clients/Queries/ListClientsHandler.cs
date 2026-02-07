using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Queries;

public class ListClientsHandler : IRequestHandler<ListClientsQuery, PaginatedList<ClientListItemDto>>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public ListClientsHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ClientListItemDto>> Handle(
        ListClientsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Client>().AsQueryable();

        // Filter archived unless explicitly included
        if (!request.IncludeArchived)
        {
            query = query.Where(c => c.DeletedAt == null);
        }

        // Filter by client type
        if (!string.IsNullOrWhiteSpace(request.ClientType) &&
            Enum.TryParse<ClientType>(request.ClientType, ignoreCase: true, out var clientType))
        {
            query = query.Where(c => c.ClientType == clientType);
        }

        // Search functionality using EF.Functions.Like for case-insensitive search
        // Note: PostgreSQL with CITEXT or proper collation handles case-insensitivity at database level
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            var searchPattern = $"%{search}%";

            // CA1307/CA1304 suppressed: EF Core translates this to SQL LIKE which doesn't use StringComparison
#pragma warning disable CA1307, CA1304
            query = query.Where(c =>
                EF.Functions.Like(c.FirstName ?? string.Empty, searchPattern) ||
                EF.Functions.Like(c.LastName ?? string.Empty, searchPattern) ||
                EF.Functions.Like(c.BusinessName ?? string.Empty, searchPattern) ||
                EF.Functions.Like(c.Email, searchPattern) ||
                c.TaxIdentifierLast4.Contains(search));
#pragma warning restore CA1307, CA1304
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Project to DTO and paginate
        var dtoQuery = query.ProjectTo<ClientListItemDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<ClientListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<Client> ApplySorting(IQueryable<Client> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "NAME" => isDescending
                ? query.OrderByDescending(c => c.ClientType == ClientType.Individual
                    ? c.LastName + ", " + c.FirstName
                    : c.BusinessName)
                : query.OrderBy(c => c.ClientType == ClientType.Individual
                    ? c.LastName + ", " + c.FirstName
                    : c.BusinessName),
            "EMAIL" => isDescending
                ? query.OrderByDescending(c => c.Email)
                : query.OrderBy(c => c.Email),
            "TYPE" => isDescending
                ? query.OrderByDescending(c => c.ClientType)
                : query.OrderBy(c => c.ClientType),
            "CREATEDAT" => isDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            "UPDATEDAT" => isDescending
                ? query.OrderByDescending(c => c.UpdatedAt)
                : query.OrderBy(c => c.UpdatedAt),
            _ => query.OrderBy(c => c.ClientType == ClientType.Individual
                ? c.LastName + ", " + c.FirstName
                : c.BusinessName)
        };
    }
}
