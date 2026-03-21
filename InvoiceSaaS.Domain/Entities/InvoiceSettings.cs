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

    public string GenerateNextInvoiceNumber()
    {
        CurrentNumber++;
        Touch();
        return $"{Prefix}{CurrentNumber}{Suffix}";
    }
}
