namespace InvoiceSaaS.Application.DTOs.Template;

public record TemplateDto(Guid Id, string Name, string TemplateJson, bool IsDefault);
public record CreateTemplateDto(string Name, string TemplateJson, bool IsDefault);
public record UpdateTemplateDto(string Name, string TemplateJson, bool IsDefault);
