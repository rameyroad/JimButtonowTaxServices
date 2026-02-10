using MediatR;
using TranscriptAnalyzer.Application.ClientApprovals.DTOs;

namespace TranscriptAnalyzer.Application.ClientApprovals.Commands.RespondToApproval;

public record RespondToApprovalCommand : IRequest<ClientApprovalDto?>
{
    public required string Token { get; init; }
    public required bool Approved { get; init; }
    public string? Notes { get; init; }
}
