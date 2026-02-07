using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.DTOs;

public record ClientDetailDto : ClientDto
{
    public string? Notes { get; init; }
    public UserSummaryDto? CreatedBy { get; init; }
    public int ActiveAuthorizationCount { get; init; }
    public int TranscriptCount { get; init; }
}

public record UserSummaryDto(
    Guid Id,
    string Name,
    string Email);
