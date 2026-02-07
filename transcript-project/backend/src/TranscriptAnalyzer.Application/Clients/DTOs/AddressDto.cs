namespace TranscriptAnalyzer.Application.Clients.DTOs;

public record AddressDto(
    string Street1,
    string? Street2,
    string City,
    string State,
    string PostalCode,
    string Country = "US");
