using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Product : BaseAuditableEntity
{
    public Product(Guid tenantId, Guid companyId, string name, decimal unitPrice, string sku, string? description = null, decimal taxPercent = 0m)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required.", nameof(sku));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        if (taxPercent < 0) throw new ArgumentOutOfRangeException(nameof(taxPercent), "Tax percent cannot be negative.");

        SetTenant(tenantId);
        CompanyId = companyId;
        Name = name.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        UnitPrice = unitPrice;
        Description = description?.Trim();
        TaxPercent = taxPercent;
    }

    private Product()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public Tenant? Tenant { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string? Description { get; private set; }
    public decimal TaxPercent { get; private set; }

    public void UpdateDetails(string name, decimal unitPrice, string sku, string? description, decimal taxPercent)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required.", nameof(sku));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        if (taxPercent < 0) throw new ArgumentOutOfRangeException(nameof(taxPercent), "Tax percent cannot be negative.");

        Name = name.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        UnitPrice = unitPrice;
        Description = description?.Trim();
        TaxPercent = taxPercent;
        Touch();
    }
}
