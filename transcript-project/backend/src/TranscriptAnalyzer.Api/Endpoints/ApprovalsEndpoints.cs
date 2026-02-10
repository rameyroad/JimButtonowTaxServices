using MediatR;
using TranscriptAnalyzer.Application.ClientApprovals.Commands.RespondToApproval;
using TranscriptAnalyzer.Application.ClientApprovals.Queries.GetApprovalByToken;

namespace TranscriptAnalyzer.Api.Endpoints;

public static class ApprovalsEndpoints
{
    public static RouteGroupBuilder MapApprovalsEndpoints(this RouteGroupBuilder group)
    {
        var approvals = group.MapGroup("/approvals")
            .WithTags("ClientApprovals");

        // GET /approvals/{token} - Get approval details (public, no auth)
        approvals.MapGet("/{token}", async (
            string token,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetApprovalByTokenQuery(token);
            var result = await sender.Send(query, cancellationToken);

            return result is null
                ? Results.NotFound(new { message = "Approval not found or link has expired." })
                : Results.Ok(result);
        })
        .WithName("GetApprovalByToken")
        .WithSummary("Get client approval details by secure token")
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /approvals/{token}/respond - Submit approval response (public, no auth)
        approvals.MapPost("/{token}/respond", async (
            string token,
            RespondToApprovalCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var commandWithToken = command with { Token = token };
            var result = await sender.Send(commandWithToken, cancellationToken);

            return result is null
                ? Results.NotFound(new { message = "Approval not found or link has expired." })
                : Results.Ok(result);
        })
        .WithName("RespondToApproval")
        .WithSummary("Submit client approval or decline response")
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
