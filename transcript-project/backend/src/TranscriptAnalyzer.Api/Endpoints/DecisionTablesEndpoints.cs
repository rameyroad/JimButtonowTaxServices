using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.Commands.AddDecisionRule;
using TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;
using TranscriptAnalyzer.Application.DecisionTables.Commands.PublishDecisionTable;
using TranscriptAnalyzer.Application.DecisionTables.Commands.RemoveDecisionRule;
using TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionRule;
using TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionTable;
using TranscriptAnalyzer.Application.DecisionTables.Queries.GetDecisionTable;
using TranscriptAnalyzer.Application.DecisionTables.Queries.ListDecisionTables;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class DecisionTablesEndpoints
{
    public static RouteGroupBuilder MapDecisionTablesEndpoints(this RouteGroupBuilder group)
    {
        var decisionTables = group.MapGroup("/decision-tables")
            .WithTags("Decision Tables")
            .RequireAuthorization("PlatformAdmin");

        // GET /decision-tables - List decision tables
        decisionTables.MapGet("/", async (
            int? page,
            int? pageSize,
            string? search,
            string? status,
            string? sortBy,
            string? sortOrder,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            PublishStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PublishStatus>(status, ignoreCase: true, out var parsed))
            {
                statusFilter = parsed;
            }

            var query = new ListDecisionTablesQuery
            {
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                Search = search,
                Status = statusFilter,
                SortBy = sortBy ?? "name",
                SortOrder = sortOrder ?? "asc"
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
        .WithName("ListDecisionTables")
        .WithSummary("List decision tables with pagination and filtering")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // POST /decision-tables - Create decision table
        decisionTables.MapPost("/", async (
            CreateDecisionTableCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/decision-tables/{result.Id}", result);
        })
        .WithName("CreateDecisionTable")
        .WithSummary("Create a new decision table")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // GET /decision-tables/{id} - Get decision table detail
        decisionTables.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDecisionTableQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetDecisionTable")
        .WithSummary("Get decision table details by ID")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /decision-tables/{id} - Update decision table
        decisionTables.MapPut("/{id:guid}", async (
            Guid id,
            UpdateDecisionTableCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateDecisionTable")
        .WithSummary("Update an existing decision table")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /decision-tables/{id}/rules - Add rule
        decisionTables.MapPost("/{id:guid}/rules", async (
            Guid id,
            AddDecisionRuleCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { DecisionTableId = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Created($"/api/v1/decision-tables/{id}/rules/{result.Id}", result);
        })
        .WithName("AddDecisionRule")
        .WithSummary("Add a rule to a decision table")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /decision-tables/{id}/rules/{ruleId} - Update rule
        decisionTables.MapPut("/{id:guid}/rules/{ruleId:guid}", async (
            Guid id,
            Guid ruleId,
            UpdateDecisionRuleCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithIds = command with { DecisionTableId = id, RuleId = ruleId };
            var result = await sender.Send(commandWithIds, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateDecisionRule")
        .WithSummary("Update a rule in a decision table")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /decision-tables/{id}/rules/{ruleId} - Remove rule
        decisionTables.MapDelete("/{id:guid}/rules/{ruleId:guid}", async (
            Guid id,
            Guid ruleId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveDecisionRuleCommand(id, ruleId);
            var found = await sender.Send(command, cancellationToken);

            return found
                ? Results.NoContent()
                : Results.NotFound();
        })
        .WithName("RemoveDecisionRule")
        .WithSummary("Remove a rule from a decision table")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /decision-tables/{id}/publish - Publish decision table
        decisionTables.MapPost("/{id:guid}/publish", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishDecisionTableCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("PublishDecisionTable")
        .WithSummary("Publish a decision table")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
