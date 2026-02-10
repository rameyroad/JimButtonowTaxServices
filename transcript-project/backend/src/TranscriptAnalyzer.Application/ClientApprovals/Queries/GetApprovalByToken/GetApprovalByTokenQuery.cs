using MediatR;
using TranscriptAnalyzer.Application.ClientApprovals.DTOs;

namespace TranscriptAnalyzer.Application.ClientApprovals.Queries.GetApprovalByToken;

public record GetApprovalByTokenQuery(string Token) : IRequest<ClientApprovalDto?>;
