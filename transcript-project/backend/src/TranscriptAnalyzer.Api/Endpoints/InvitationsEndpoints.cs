using MediatR;
using TranscriptAnalyzer.Application.Invitations.Commands.CreateInvitation;
using TranscriptAnalyzer.Application.Invitations.Queries.ListInvitations;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class InvitationsEndpoints
{
    public static RouteGroupBuilder MapInvitationsEndpoints(this RouteGroupBuilder group)
    {
        var invitations = group.MapGroup("/invitations")
            .WithTags("Invitations")
            .RequireAuthorization("AdminOnly");

        // GET /invitations - List invitations for the current organization
        invitations.MapGet("/", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new ListInvitationsQuery();
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("ListInvitations")
        .WithSummary("List invitations for the current organization")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        // POST /invitations - Create a new invitation
        invitations.MapPost("/", async (
            CreateInvitationCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/invitations/{result.Id}", result);
        })
        .WithName("CreateInvitation")
        .WithSummary("Invite a team member to the organization")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

        return group;
    }
}
