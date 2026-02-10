using MediatR;
using TranscriptAnalyzer.Application.Workflows.Commands.CancelWorkflow;
using TranscriptAnalyzer.Application.Workflows.Commands.StartWorkflow;
using TranscriptAnalyzer.Application.Workflows.Queries.GetCaseWorkflow;
using TranscriptAnalyzer.Application.Workflows.Queries.ListCaseWorkflows;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class CaseWorkflowsEndpoints
{
    public static RouteGroupBuilder MapCaseWorkflowsEndpoints(this RouteGroupBuilder group)
    {
        var caseWorkflows = group.MapGroup("/clients/{clientId:guid}/workflows")
            .WithTags("Case Workflows")
            .RequireAuthorization();

        // GET /clients/{clientId}/workflows - List case workflows for client
        caseWorkflows.MapGet("/", async (
            Guid clientId,
            int? page,
            int? pageSize,
            string? status,
            string? sortBy,
            string? sortOrder,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            WorkflowExecutionStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<WorkflowExecutionStatus>(status, ignoreCase: true, out var parsed))
            {
                statusFilter = parsed;
            }

            var query = new ListCaseWorkflowsQuery
            {
                ClientId = clientId,
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                Status = statusFilter,
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
        .WithName("ListCaseWorkflows")
        .WithSummary("List case workflows for a client")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /clients/{clientId}/workflows - Start workflow on client
        caseWorkflows.MapPost("/", async (
            Guid clientId,
            StartWorkflowCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithClientId = command with { ClientId = clientId };
            var result = await sender.Send(commandWithClientId, cancellationToken);

            return Results.Created(
                $"/api/v1/clients/{clientId}/workflows/{result.Id}",
                result);
        })
        .WithName("StartWorkflow")
        .WithSummary("Start a workflow on a client")
        .RequireAuthorization("WriteClients")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // GET /clients/{clientId}/workflows/{caseId} - Get case workflow detail
        caseWorkflows.MapGet("/{caseId:guid}", async (
            Guid clientId,
            Guid caseId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCaseWorkflowQuery(clientId, caseId);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetCaseWorkflow")
        .WithSummary("Get case workflow details")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // POST /clients/{clientId}/workflows/{caseId}/cancel - Cancel workflow
        caseWorkflows.MapPost("/{caseId:guid}/cancel", async (
            Guid clientId,
            Guid caseId,
            CancelWorkflowRequest? request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelWorkflowCommand
            {
                ClientId = clientId,
                CaseWorkflowId = caseId,
                Reason = request?.Reason
            };

            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("CancelCaseWorkflow")
        .WithSummary("Cancel a running case workflow")
        .RequireAuthorization("WriteClients")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}

public record CancelWorkflowRequest
{
    public string? Reason { get; init; }
}
