using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class TaxDefinition : BaseEntity
{
    public TaxDefinition(Guid tenantId, string name, decimal rate, bool isCompound = false, int priority = 0)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tax name is required.", nameof(name));
        if (rate < 0) throw new ArgumentException("Rate cannot be negative.", nameof(rate));

        SetTenant(tenantId);
        Name = name.Trim();
        Rate = rate;
        IsCompound = isCompound;
        Priority = priority;
    }

    private TaxDefinition() { }

    public string Name { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public bool IsCompound { get; private set; }
    public int Priority { get; private set; }

    public void Update(string name, decimal rate, bool isCompound, int priority)
    {
        Name = name?.Trim() ?? Name;
        Rate = rate;
        IsCompound = isCompound;
        Priority = priority;
        Touch();
    }
}
