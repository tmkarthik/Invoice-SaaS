using FluentValidation;
using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Validators;

public class AddInvoiceItemDtoValidator : AbstractValidator<AddInvoiceItemDto>
{
    public AddInvoiceItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than zero.");
        RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.");
    }
}

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is required.");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.");
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Invoice must have items.")
            .Must(x => x != null && x.Count > 0).WithMessage("Invoice must contain at least one item.");
        RuleForEach(x => x.Items).SetValidator(new AddInvoiceItemDtoValidator());
    }
}
