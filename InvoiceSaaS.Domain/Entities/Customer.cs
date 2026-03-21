using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Customer : BaseEntity
{
    private readonly List<Invoice> _invoices = [];

    public Customer(Guid tenantId, Guid companyId, string displayName, string email, string? gstNumber = null)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name is required.", nameof(displayName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        SetTenant(tenantId);
        CompanyId = companyId;
        DisplayName = displayName.Trim();
        Email = email.Trim().ToLowerInvariant();
        GstNumber = gstNumber?.Trim();
    }

    private Customer()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public Tenant? Tenant { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? GstNumber { get; private set; }
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    public void UpdateDetails(string displayName, string email, string? gstNumber)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name is required.", nameof(displayName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        
        DisplayName = displayName.Trim();
        Email = email.Trim().ToLowerInvariant();
        GstNumber = gstNumber?.Trim();
        Touch();
    }
}
