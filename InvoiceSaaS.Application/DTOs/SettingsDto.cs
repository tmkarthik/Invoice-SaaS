namespace InvoiceSaaS.Application.DTOs;

public record InvoiceSettingsDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Prefix { get; init; } = string.Empty;
    public string? Suffix { get; init; }
    public int NextInvoiceNumber { get; init; }
    public string DefaultCurrency { get; init; } = "USD";
    public decimal DefaultTaxRate { get; init; }
    public int DefaultDueDays { get; init; } = 30;
    public string? LogoUrl { get; init; }
}

public record UpdateSettingsDto
{
    public string Prefix { get; init; } = string.Empty;
    public string? Suffix { get; init; }
    public int NextInvoiceNumber { get; init; }
    public string DefaultCurrency { get; init; } = "USD";
    public decimal DefaultTaxRate { get; init; }
    public int DefaultDueDays { get; init; } = 30;
    public string? LogoUrl { get; init; }
}
