using MediatR;
using TranscriptAnalyzer.Application.HumanTasks.Commands.CompleteHumanTask;
using TranscriptAnalyzer.Application.HumanTasks.Commands.ReassignHumanTask;
using TranscriptAnalyzer.Application.HumanTasks.Queries.GetHumanTask;
using TranscriptAnalyzer.Application.HumanTasks.Queries.ListHumanTasks;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class HumanTasksEndpoints
{
    public static RouteGroupBuilder MapHumanTasksEndpoints(this RouteGroupBuilder group)
    {
        var tasks = group.MapGroup("/tasks")
            .WithTags("HumanTasks")
            .RequireAuthorization();

        // GET /tasks - List human tasks for the current tenant
        tasks.MapGet("/", async (
            int? page,
            int? pageSize,
            string? status,
            Guid? assignedToUserId,
            string? sortBy,
            string? sortOrder,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            HumanTaskStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<HumanTaskStatus>(status, ignoreCase: true, out var parsed))
            {
                statusFilter = parsed;
            }

            var query = new ListHumanTasksQuery
            {
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                Status = statusFilter,
                AssignedToUserId = assignedToUserId,
                SortBy = sortBy ?? "createdAt",
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
        .WithName("ListHumanTasks")
        .WithSummary("List human tasks with pagination and filtering")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // GET /tasks/{id} - Get human task detail
        tasks.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetHumanTaskQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetHumanTask")
        .WithSummary("Get human task details by ID")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // POST /tasks/{id}/complete - Complete a human task with decision
        tasks.MapPost("/{id:guid}/complete", async (
            Guid id,
            CompleteHumanTaskCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("CompleteHumanTask")
        .WithSummary("Complete a human task with a decision")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // POST /tasks/{id}/reassign - Reassign a human task
        tasks.MapPost("/{id:guid}/reassign", async (
            Guid id,
            ReassignHumanTaskCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("ReassignHumanTask")
        .WithSummary("Reassign a human task to another user")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
