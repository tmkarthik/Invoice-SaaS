using FluentValidation;
using InvoiceSaaS.Application.DTOs.Customer;

namespace InvoiceSaaS.Application.Validators.Customer;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().WithMessage("Display name is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                             .EmailAddress().WithMessage("Invalid email format.");
    }
}
