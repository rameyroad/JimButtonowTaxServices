using MediatR;

namespace TranscriptAnalyzer.Application.Registration.Commands.RegisterOrganization;

public record RegisterOrganizationCommand : IRequest<RegisterOrganizationResult>
{
    public required string OrganizationName { get; init; }
    public required string ContactEmail { get; init; }
    public required string Street1 { get; init; }
    public string? Street2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string PostalCode { get; init; }
    public required string AdminFirstName { get; init; }
    public required string AdminLastName { get; init; }
    public required string AdminEmail { get; init; }
}

public record RegisterOrganizationResult
{
    public required Guid OrganizationId { get; init; }
    public required Guid UserId { get; init; }
    public required string OrganizationName { get; init; }
}
