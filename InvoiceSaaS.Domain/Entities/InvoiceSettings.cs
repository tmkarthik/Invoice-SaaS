using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class InvoiceSettings : BaseEntity
{
    public InvoiceSettings(Guid tenantId, Guid companyId, string prefix, int startNumber)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId is required.", nameof(companyId));

        SetTenant(tenantId);
        CompanyId = companyId;
        Prefix = prefix?.Trim() ?? string.Empty;
        CurrentNumber = startNumber;
    }

    private InvoiceSettings() { }

    public Guid CompanyId { get; private set; }
    public string Prefix { get; private set; } = string.Empty;
    public int CurrentNumber { get; private set; }
    public string? Suffix { get; private set; }

    public string DefaultCurrency { get; private set; } = "USD";
    public decimal DefaultTaxRate { get; private set; } = 0;
    public int DefaultDueDays { get; private set; } = 30;
    public string? LogoUrl { get; private set; }

    public void Update(string prefix, string? suffix, int nextNumber, string currency, decimal taxRate, int dueDays, string? logoUrl)
    {
        Prefix = prefix?.Trim() ?? string.Empty;
        Suffix = suffix?.Trim();
        CurrentNumber = nextNumber - 1; // GenerateNext increments this
        DefaultCurrency = currency?.Trim().ToUpper() ?? "USD";
        DefaultTaxRate = taxRate;
        DefaultDueDays = dueDays;
        LogoUrl = logoUrl?.Trim();
        Touch();
    }

    public string GenerateNextInvoiceNumber()
    {
        CurrentNumber++;
        Touch();
        return $"{Prefix}{CurrentNumber}{Suffix}";
    }
}
