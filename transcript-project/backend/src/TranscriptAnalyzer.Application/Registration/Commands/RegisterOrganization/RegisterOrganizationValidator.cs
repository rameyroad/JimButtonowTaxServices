using FluentValidation;

namespace TranscriptAnalyzer.Application.Registration.Commands.RegisterOrganization;

public class RegisterOrganizationValidator : AbstractValidator<RegisterOrganizationCommand>
{
    public RegisterOrganizationValidator()
    {
        RuleFor(x => x.OrganizationName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Street1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(2);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.AdminFirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AdminLastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
