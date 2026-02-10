using MediatR;
using TranscriptAnalyzer.Application.Invitations.Commands.AcceptInvitation;
using TranscriptAnalyzer.Application.Registration.Commands.RegisterOrganization;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class RegistrationEndpoints
{
    public static RouteGroupBuilder MapRegistrationEndpoints(this RouteGroupBuilder group)
    {
        // POST /register - Create a new organization and admin user (anonymous)
        group.MapPost("/register", async (
            RegisterOrganizationCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/organizations/{result.OrganizationId}", result);
        })
        .WithName("RegisterOrganization")
        .WithSummary("Register a new subscriber organization with admin user")
        .WithTags("Registration")
        .AllowAnonymous()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /invitations/{token}/accept - Accept an invitation (anonymous)
        group.MapPost("/invitations/{token}/accept", async (
            string token,
            AcceptInvitationCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithToken = command with { Token = token };
            var result = await sender.Send(commandWithToken, cancellationToken);

            return result is null
                ? Results.NotFound(new { message = "Invalid or expired invitation." })
                : Results.Ok(result);
        })
        .WithName("AcceptInvitation")
        .WithSummary("Accept a team invitation and create user account")
        .WithTags("Registration")
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
