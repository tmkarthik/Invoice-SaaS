using FluentValidation;
using InvoiceSaaS.Application.DTOs.Template;

namespace InvoiceSaaS.Application.Validators.Template;

public class CreateTemplateDtoValidator : AbstractValidator<CreateTemplateDto>
{
    public CreateTemplateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Template name is required.");
        RuleFor(x => x.TemplateJson).NotEmpty().WithMessage("Template JSON is required.");
    }
}

public class UpdateTemplateDtoValidator : AbstractValidator<UpdateTemplateDto>
{
    public UpdateTemplateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Template name is required.");
        RuleFor(x => x.TemplateJson).NotEmpty().WithMessage("Template JSON is required.");
    }
}
