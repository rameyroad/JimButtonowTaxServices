using MediatR;
using TranscriptAnalyzer.Application.Clients.Commands.ArchiveClient;
using TranscriptAnalyzer.Application.Clients.Commands.CreateClient;
using TranscriptAnalyzer.Application.Clients.Commands.RestoreClient;
using TranscriptAnalyzer.Application.Clients.Commands.UpdateClient;
using TranscriptAnalyzer.Application.Clients.Queries;
using TranscriptAnalyzer.Application.Clients.Queries.GetClient;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class ClientsEndpoints
{
    public static RouteGroupBuilder MapClientsEndpoints(this RouteGroupBuilder group)
    {
        var clients = group.MapGroup("/clients")
            .WithTags("Clients")
            .RequireAuthorization();

        // GET /clients - List clients with pagination, search, and filtering
        clients.MapGet("/", async (
            int? page,
            int? pageSize,
            string? search,
            string? clientType,
            string? sortBy,
            string? sortOrder,
            bool? includeArchived,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListClientsQuery
            {
                Page = page ?? 1,
                PageSize = pageSize ?? 20,
                Search = search,
                ClientType = clientType,
                SortBy = sortBy ?? "name",
                SortOrder = sortOrder ?? "asc",
                IncludeArchived = includeArchived ?? false
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
        .WithName("ListClients")
        .WithSummary("List clients with pagination and filtering")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /clients - Create client (US2/US3)
        clients.MapPost("/", async (
            CreateClientCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/clients/{result.Id}", result);
        })
        .WithName("CreateClient")
        .WithSummary("Create a new client (individual or business)")
        .RequireAuthorization("WriteClients")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status409Conflict);

        // GET /clients/{id} - Get client details (US4)
        clients.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetClientQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetClient")
        .WithSummary("Get client details by ID")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // PATCH /clients/{id} - Update client (US4)
        clients.MapPatch("/{id:guid}", async (
            Guid id,
            UpdateClientCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            // Ensure the ID in the URL matches the command
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateClient")
        .WithSummary("Update an existing client")
        .RequireAuthorization("WriteClients")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        // DELETE /clients/{id} - Archive client (US6)
        clients.MapDelete("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new ArchiveClientCommand(id);
            var found = await sender.Send(command, cancellationToken);

            return found
                ? Results.NoContent()
                : Results.NotFound();
        })
        .WithName("ArchiveClient")
        .WithSummary("Archive (soft delete) a client")
        .RequireAuthorization("AdminOnly")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /clients/{id}/restore - Restore client (US6)
        clients.MapPost("/{id:guid}/restore", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RestoreClientCommand(id);
            var result = await sender.Send(command, cancellationToken);

            if (!result.Found)
            {
                return Results.NotFound();
            }

            if (!result.WasArchived)
            {
                return Results.BadRequest(new { detail = "Client is not archived" });
            }

            return Results.Ok(result.Client);
        })
        .WithName("RestoreClient")
        .WithSummary("Restore an archived client")
        .RequireAuthorization("AdminOnly")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
