namespace TranscriptAnalyzer.Domain.ValueObjects;

public sealed record Address
{
    public string Street1 { get; }
    public string? Street2 { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address()
    {
        Street1 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        PostalCode = string.Empty;
        Country = "US";
    }

    public Address(
        string street1,
        string? street2,
        string city,
        string state,
        string postalCode,
        string country = "US")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street1);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        Street1 = street1.Trim();
        Street2 = street2?.Trim();
        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        PostalCode = postalCode.Trim();
        Country = country.Trim().ToUpperInvariant();
    }

    public string ToSingleLine()
    {
        var parts = new List<string> { Street1 };
        if (!string.IsNullOrWhiteSpace(Street2))
        {
            parts.Add(Street2);
        }
        parts.Add($"{City}, {State} {PostalCode}");
        if (Country != "US")
        {
            parts.Add(Country);
        }
        return string.Join(", ", parts);
    }

    public override string ToString() => ToSingleLine();
}
