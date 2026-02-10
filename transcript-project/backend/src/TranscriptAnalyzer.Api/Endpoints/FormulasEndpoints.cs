using MediatR;
using TranscriptAnalyzer.Application.Formulas.Commands.CreateFormula;
using TranscriptAnalyzer.Application.Formulas.Commands.PublishFormula;
using TranscriptAnalyzer.Application.Formulas.Commands.UnpublishFormula;
using TranscriptAnalyzer.Application.Formulas.Commands.UpdateFormula;
using TranscriptAnalyzer.Application.Formulas.Queries.GetFormula;
using TranscriptAnalyzer.Application.Formulas.Queries.ListFormulas;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class FormulasEndpoints
{
    public static RouteGroupBuilder MapFormulasEndpoints(this RouteGroupBuilder group)
    {
        var formulas = group.MapGroup("/formulas")
            .WithTags("Formulas")
            .RequireAuthorization("PlatformAdmin");

        // GET /formulas - List formulas
        formulas.MapGet("/", async (
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

            var query = new ListFormulasQuery
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
        .WithName("ListFormulas")
        .WithSummary("List calculation formulas")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // POST /formulas - Create formula
        formulas.MapPost("/", async (
            CreateFormulaCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/v1/formulas/{result.Id}", result);
        })
        .WithName("CreateFormula")
        .WithSummary("Create a new calculation formula")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // GET /formulas/{id} - Get formula detail
        formulas.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFormulaQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetFormula")
        .WithSummary("Get formula details by ID")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /formulas/{id} - Update formula
        formulas.MapPut("/{id:guid}", async (
            Guid id,
            UpdateFormulaCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithId = command with { Id = id };
            var result = await sender.Send(commandWithId, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UpdateFormula")
        .WithSummary("Update an existing formula")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /formulas/{id}/publish - Publish formula
        formulas.MapPost("/{id:guid}/publish", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishFormulaCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("PublishFormula")
        .WithSummary("Publish a formula")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        // POST /formulas/{id}/unpublish - Unpublish formula
        formulas.MapPost("/{id:guid}/unpublish", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UnpublishFormulaCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("UnpublishFormula")
        .WithSummary("Unpublish a formula")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
