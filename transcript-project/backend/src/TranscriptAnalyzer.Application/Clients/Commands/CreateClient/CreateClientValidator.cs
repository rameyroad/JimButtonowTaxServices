using System.Text.RegularExpressions;
using FluentValidation;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Commands.CreateClient;

public partial class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    // SSN format: XXX-XX-XXXX or XXXXXXXXX (9 digits)
    [GeneratedRegex(@"^(?!000|666|9\d{2})\d{3}[-]?(?!00)\d{2}[-]?(?!0000)\d{4}$")]
    private static partial Regex SsnRegex();

    // EIN format: XX-XXXXXXX or XXXXXXXXX (9 digits)
    [GeneratedRegex(@"^\d{2}[-]?\d{7}$")]
    private static partial Regex EinRegex();

    public CreateClientValidator()
    {
        // Common validations
        RuleFor(x => x.TaxIdentifier)
            .NotEmpty().WithMessage("Tax identifier is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Phone)
            .Matches(@"^[\d\s\-\(\)\+\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone must contain only digits, spaces, dashes, parentheses, plus signs, and periods");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required")
            .SetValidator(new AddressValidator());

        // Individual-specific validations
        When(x => x.ClientType == ClientType.Individual, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required for individual clients")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required for individual clients")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.TaxIdentifier)
                .Must(BeValidSsn).WithMessage("SSN must be in format XXX-XX-XXXX or XXXXXXXXX");
        });

        // Business-specific validations
        When(x => x.ClientType == ClientType.Business, () =>
        {
            RuleFor(x => x.BusinessName)
                .NotEmpty().WithMessage("Business name is required for business clients")
                .MaximumLength(200).WithMessage("Business name must not exceed 200 characters");

            RuleFor(x => x.EntityType)
                .NotNull().WithMessage("Entity type is required for business clients");

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

public class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(x => x.Street1)
            .NotEmpty().WithMessage("Street address is required")
            .MaximumLength(200).WithMessage("Street address must not exceed 200 characters");

        RuleFor(x => x.Street2)
            .MaximumLength(200).WithMessage("Street address 2 must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Street2));

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(50).WithMessage("State must not exceed 50 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(50).WithMessage("Country must not exceed 50 characters");
    }
}
