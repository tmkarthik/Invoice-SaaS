using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Customer : BaseEntity
{
    private readonly List<Invoice> _invoices = [];
    private readonly List<CustomerAddress> _customerAddresses = [];

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
    public IReadOnlyCollection<CustomerAddress> CustomerAddresses => _customerAddresses.AsReadOnly();

    public void AddAddress(CustomerAddress address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));
        if (address.TenantId != TenantId) throw new InvalidOperationException("Address TenantId mismatch.");
        
        // Prevent duplicates (Customer + Address + Type combination)
        if (_customerAddresses.Any(a => a.AddressId == address.AddressId && a.AddressType == address.AddressType))
        {
            return;
        }

        _customerAddresses.Add(address);
        Touch();
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _customerAddresses.FirstOrDefault(a => a.AddressId == addressId);
        if (address != null)
        {
            _customerAddresses.Remove(address);
            Touch();
        }
    }

    public void AddBillingAddress(Address address, bool isDefault = true)
    {
        if (address.TenantId != TenantId) throw new InvalidOperationException("Address TenantId mismatch.");

        if (isDefault)
        {
            foreach (var ca in _customerAddresses.Where(a => a.AddressType == Enums.AddressType.Billing))
            {
                ca.SetDefault(false);
            }
        }

        AddAddress(new CustomerAddress(TenantId, Id, address.Id, Enums.AddressType.Billing, isDefault));
    }

    public void AddShippingAddress(Address address, bool isDefault = true)
    {
        if (address.TenantId != TenantId) throw new InvalidOperationException("Address TenantId mismatch.");

        if (isDefault)
        {
            foreach (var ca in _customerAddresses.Where(a => a.AddressType == Enums.AddressType.Shipping))
            {
                ca.SetDefault(false);
            }
        }

        AddAddress(new CustomerAddress(TenantId, Id, address.Id, Enums.AddressType.Shipping, isDefault));
    }

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
