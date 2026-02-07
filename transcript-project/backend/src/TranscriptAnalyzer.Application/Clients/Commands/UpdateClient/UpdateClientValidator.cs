using FluentValidation;
using TranscriptAnalyzer.Application.Clients.DTOs;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateClient;

public class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Client ID is required");

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("Version is required for optimistic concurrency");

        // Individual fields (optional but validated if provided)
        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        // Business fields (optional but validated if provided)
        RuleFor(x => x.BusinessName)
            .MaximumLength(200).WithMessage("Business name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.BusinessName));

        RuleFor(x => x.ResponsibleParty)
            .MaximumLength(200).WithMessage("Responsible party must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ResponsibleParty));

        // Common fields (optional but validated if provided)
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^[\d\s\-\(\)\+\.]+$").WithMessage("Phone must contain only digits, spaces, dashes, parentheses, plus signs, and periods")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        // Address validation (if provided)
        RuleFor(x => x.Address)
            .SetValidator(new UpdateAddressValidator()!)
            .When(x => x.Address != null);
    }
}

public class UpdateAddressValidator : AbstractValidator<AddressDto>
{
    public UpdateAddressValidator()
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
