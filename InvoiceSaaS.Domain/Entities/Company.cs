using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Company : BaseEntity
{
    private readonly List<User> _users = [];
    private readonly List<Customer> _customers = [];
    private readonly List<Product> _products = [];
    private readonly List<Template> _templates = [];
    private readonly List<Invoice> _invoices = [];

    public Company(Guid tenantId, string legalName, string taxNumber)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(legalName)) throw new ArgumentException("Legal name is required.", nameof(legalName));
        if (string.IsNullOrWhiteSpace(taxNumber)) throw new ArgumentException("Tax number is required.", nameof(taxNumber));

        SetTenant(tenantId);
        LegalName = legalName.Trim();
        TaxNumber = taxNumber.Trim();
    }

    private Company()
    {
    }

    public string LegalName { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public Tenant? Tenant { get; private set; }
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Customer> Customers => _customers.AsReadOnly();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public IReadOnlyCollection<Template> Templates => _templates.AsReadOnly();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();
}
