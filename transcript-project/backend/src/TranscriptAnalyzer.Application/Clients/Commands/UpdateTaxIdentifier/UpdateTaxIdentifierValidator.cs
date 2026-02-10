using System.Text.RegularExpressions;
using FluentValidation;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateTaxIdentifier;

public partial class UpdateTaxIdentifierValidator : AbstractValidator<UpdateTaxIdentifierCommand>
{
    // SSN format: XXX-XX-XXXX or XXXXXXXXX (9 digits)
    [GeneratedRegex(@"^(?!000|666|9\d{2})\d{3}[-]?(?!00)\d{2}[-]?(?!0000)\d{4}$")]
    private static partial Regex SsnRegex();

    // EIN format: XX-XXXXXXX or XXXXXXXXX (9 digits)
    [GeneratedRegex(@"^\d{2}[-]?\d{7}$")]
    private static partial Regex EinRegex();

    public UpdateTaxIdentifierValidator()
    {
        RuleFor(x => x.TaxIdentifier)
            .NotEmpty().WithMessage("Tax identifier is required");

        RuleFor(x => x.Version)
            .GreaterThanOrEqualTo(0).WithMessage("Version is required");

        When(x => x.ClientType == ClientType.Individual, () =>
        {
            RuleFor(x => x.TaxIdentifier)
                .Must(BeValidSsn).WithMessage("SSN must be in format XXX-XX-XXXX or XXXXXXXXX");
        });

        When(x => x.ClientType == ClientType.Business, () =>
        {
            RuleFor(x => x.TaxIdentifier)
                .Must(BeValidEin).WithMessage("EIN must be in format XX-XXXXXXX or XXXXXXXXX");
        });
    }

    private static bool BeValidSsn(string? ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
            return false;

        return SsnRegex().IsMatch(ssn);
    }

    private static bool BeValidEin(string? ein)
    {
        if (string.IsNullOrWhiteSpace(ein))
            return false;

        return EinRegex().IsMatch(ein);
    }
}
