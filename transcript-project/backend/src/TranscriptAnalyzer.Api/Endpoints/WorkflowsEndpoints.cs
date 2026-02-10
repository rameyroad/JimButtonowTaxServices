using MediatR;
using Microsoft.AspNetCore.Authorization;
using TranscriptAnalyzer.Application.Workflows.Commands.AddWorkflowStep;
using TranscriptAnalyzer.Application.Workflows.Commands.CreateWorkflowDefinition;
using TranscriptAnalyzer.Application.Workflows.Commands.PublishWorkflow;
using TranscriptAnalyzer.Application.Workflows.Commands.RemoveWorkflowStep;
using TranscriptAnalyzer.Application.Workflows.Commands.UnpublishWorkflow;
using TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowDefinition;
using TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowStep;
using TranscriptAnalyzer.Application.Workflows.Queries.GetWorkflowDefinition;
using TranscriptAnalyzer.Application.Workflows.Queries.GetWorkflowVersion;
using TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowDefinitions;
using TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowVersions;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class WorkflowsEndpoints
{
    public static RouteGroupBuilder MapWorkflowsEndpoints(this RouteGroupBuilder group)
    {
        var workflows = group.MapGroup("/workflows")
            .WithTags("Workflows")
            .RequireAuthorization();

        // GET /workflows - List workflow definitions
        // PlatformAdmin sees all; subscribers see published only
        workflows.MapGet("/", async (
            int? page,
            int? pageSize,
            string? search,
            string? status,
            string? category,
            string? sortBy,
            string? sortOrder,
            ISender sender,
            IAuthorizationService authService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            PublishStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<PublishStatus>(status, ignoreCase: true, out var parsed))
            {
                statusFilter = parsed;
            }

            // Check if user is PlatformAdmin — if not, only show published workflows
            var authResult = await authService.AuthorizeAsync(httpContext.User, "PlatformAdmin");
            var publishedOnly = !authResult.Succeeded;

            var query = new ListWorkflowDefinitionsQuery
            {
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                Search = search,
                Status = statusFilter,
                Category = category,
                SortBy = sortBy ?? "name",
                SortOrder = sortOrder ?? "asc",
                PublishedOnly = publishedOnly
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
        .WithName("ListWorkflows")
        .WithSummary("List workflow definitions with pagination and filtering")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /workflows - Create workflow definition
        workflows.MapPost("/", async (
            CreateWorkflowDefinitionCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/workflows/{result.Id}", result);
        })
        .WithName("CreateWorkflow")
        .WithSummary("Create a new workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // GET /workflows/{id} - Get workflow definition detail
        workflows.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetWorkflowDefinitionQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetWorkflow")
        .WithSummary("Get workflow definition details by ID")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /workflows/{id} - Update workflow definition
        workflows.MapPut("/{id:guid}", async (
            Guid id,
            UpdateWorkflowDefinitionCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateWorkflow")
        .WithSummary("Update an existing workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /workflows/{id}/steps - Add step
        workflows.MapPost("/{id:guid}/steps", async (
            Guid id,
            AddWorkflowStepCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { WorkflowDefinitionId = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Created($"/api/v1/workflows/{id}/steps/{result.Id}", result);
        })
        .WithName("AddWorkflowStep")
        .WithSummary("Add a step to a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /workflows/{id}/steps/{stepId} - Update step
        workflows.MapPut("/{id:guid}/steps/{stepId:guid}", async (
            Guid id,
            Guid stepId,
            UpdateWorkflowStepCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithIds = command with { WorkflowDefinitionId = id, StepId = stepId };
            var result = await sender.Send(commandWithIds, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateWorkflowStep")
        .WithSummary("Update a step in a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /workflows/{id}/steps/{stepId} - Remove step
        workflows.MapDelete("/{id:guid}/steps/{stepId:guid}", async (
            Guid id,
            Guid stepId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveWorkflowStepCommand(id, stepId);
            var found = await sender.Send(command, cancellationToken);

            return found
                ? Results.NoContent()
                : Results.NotFound();
        })
        .WithName("RemoveWorkflowStep")
        .WithSummary("Remove a step from a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /workflows/{id}/publish - Publish workflow
        workflows.MapPost("/{id:guid}/publish", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishWorkflowCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("PublishWorkflow")
        .WithSummary("Publish a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /workflows/{id}/unpublish - Unpublish workflow
        workflows.MapPost("/{id:guid}/unpublish", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UnpublishWorkflowCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UnpublishWorkflow")
        .WithSummary("Unpublish a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // GET /workflows/{id}/versions - List versions for a workflow
        workflows.MapGet("/{id:guid}/versions", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListWorkflowVersionsQuery(id);
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ListWorkflowVersions")
        .WithSummary("List version history for a workflow definition")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // GET /workflows/{id}/versions/{versionId} - Get version detail
        workflows.MapGet("/{id:guid}/versions/{versionId:guid}", async (
            Guid id,
            Guid versionId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetWorkflowVersionQuery(id, versionId);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetWorkflowVersion")
        .WithSummary("Get version detail with snapshot data")
        .RequireAuthorization("PlatformAdmin")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
