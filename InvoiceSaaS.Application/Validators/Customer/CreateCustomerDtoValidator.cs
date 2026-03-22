using FluentValidation;
using InvoiceSaaS.Application.DTOs.Customer;

namespace InvoiceSaaS.Application.Validators.Customer;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId is required.");
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                             .EmailAddress().WithMessage("Invalid email format.");
    }
}
