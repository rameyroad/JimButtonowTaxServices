using MediatR;
using TranscriptAnalyzer.Application.Issues.Commands.UpdateIssue;
using TranscriptAnalyzer.Application.Issues.Queries.GetIssue;
using TranscriptAnalyzer.Application.Issues.Queries.ListClientIssues;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class IssuesEndpoints
{
    public static RouteGroupBuilder MapIssuesEndpoints(this RouteGroupBuilder group)
    {
        var issues = group.MapGroup("/clients/{clientId:guid}/issues")
            .WithTags("Issues")
            .RequireAuthorization();

        // GET /clients/{clientId}/issues - List issues for client
        issues.MapGet("/", async (
            Guid clientId,
            int? page,
            int? pageSize,
            string? issueType,
            string? severity,
            int? taxYear,
            bool? resolved,
            string? sortBy,
            string? sortOrder,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            IssueType? issueTypeFilter = null;
            if (!string.IsNullOrWhiteSpace(issueType) &&
                Enum.TryParse<IssueType>(issueType, ignoreCase: true, out var parsedType))
            {
                issueTypeFilter = parsedType;
            }

            IssueSeverity? severityFilter = null;
            if (!string.IsNullOrWhiteSpace(severity) &&
                Enum.TryParse<IssueSeverity>(severity, ignoreCase: true, out var parsedSeverity))
            {
                severityFilter = parsedSeverity;
            }

            var query = new ListClientIssuesQuery
            {
                ClientId = clientId,
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                IssueType = issueTypeFilter,
                Severity = severityFilter,
                TaxYear = taxYear,
                Resolved = resolved,
                SortBy = sortBy ?? "detectedAt",
                SortOrder = sortOrder ?? "desc"
            };

            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(new
            {
                result.Items,
                result.TotalCount,
                result.Page,
                result.PageSize,
                result.TotalPages,
                result.HasNextPage,
                result.HasPreviousPage
            });
        })
        .WithName("ListClientIssues")
        .WithSummary("List issues for a client")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // GET /clients/{clientId}/issues/{issueId} - Get issue detail
        issues.MapGet("/{issueId:guid}", async (
            Guid clientId,
            Guid issueId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetIssueQuery(clientId, issueId);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetIssue")
        .WithSummary("Get issue details")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // PATCH /clients/{clientId}/issues/{issueId} - Update/resolve issue
        issues.MapPatch("/{issueId:guid}", async (
            Guid clientId,
            Guid issueId,
            UpdateIssueCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithIds = command with
            {
                ClientId = clientId,
                IssueId = issueId
            };

            var result = await sender.Send(commandWithIds, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateIssue")
        .WithSummary("Update or resolve an issue")
        .RequireAuthorization("WriteClients")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
