using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Company : BaseEntity
{
    private readonly List<User> _users = [];
    private readonly List<Customer> _customers = [];
    private readonly List<Product> _products = [];
    private readonly List<Template> _templates = [];
    private readonly List<Invoice> _invoices = [];

    public Company(Guid tenantId, string legalName, string taxNumber, string email, string? phone = null, string currency = "USD", string timeZone = "UTC")
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(legalName)) throw new ArgumentException("Legal name is required.", nameof(legalName));
        if (string.IsNullOrWhiteSpace(taxNumber)) throw new ArgumentException("Tax number is required.", nameof(taxNumber));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        SetTenant(tenantId);
        LegalName = legalName.Trim();
        TaxNumber = taxNumber.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        TimeZone = timeZone.Trim();
    }

    private Company()
    {
    }

    public string LegalName { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string TimeZone { get; private set; } = "UTC";
    public Tenant? Tenant { get; private set; }
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Customer> Customers => _customers.AsReadOnly();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public IReadOnlyCollection<Template> Templates => _templates.AsReadOnly();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();
}
