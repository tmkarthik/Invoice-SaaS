using FluentValidation;
using InvoiceSaaS.Application.DTOs.Tenant;

namespace InvoiceSaaS.Application.Validators.Tenant;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class UpgradeTenantRequestValidator : AbstractValidator<UpgradeTenantRequest>
{
    public UpgradeTenantRequestValidator()
    {
        RuleFor(x => x.PlanName).NotEmpty();
        RuleFor(x => x.MaxUsers).GreaterThan(0);
        RuleFor(x => x.MaxInvoices).GreaterThan(0);
    }
}
