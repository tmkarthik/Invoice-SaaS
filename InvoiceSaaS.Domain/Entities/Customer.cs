using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Customer : BaseAuditableEntity
{
    private readonly List<Invoice> _invoices = [];
    private readonly List<CustomerAddress> _customerAddresses = [];

    public Customer(Guid tenantId, Guid companyId, string name, string email, string? phone = null, string? gstNumber = null)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        SetTenant(tenantId);
        CompanyId = companyId;
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        GstNumber = gstNumber?.Trim();
    }

    private Customer()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public Tenant? Tenant { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
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

    public CustomerAddress? GetDefaultAddress(AddressType type)
    {
        return _customerAddresses.FirstOrDefault(a => a.AddressType == type && a.IsDefault);
    }

    public void AddBillingAddress(Address address, bool isDefault = true)
    {
        if (address.TenantId != TenantId) throw new InvalidOperationException("Address TenantId mismatch.");

        if (isDefault)
        {
            foreach (var ca in _customerAddresses.Where(a => a.AddressType == Enums.AddressType.Billing))
            {
                ca.RemoveDefault();
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
                ca.RemoveDefault();
            }
        }

        AddAddress(new CustomerAddress(TenantId, Id, address.Id, Enums.AddressType.Shipping, isDefault));
    }

    public void UpdateDetails(string name, string email, string? phone, string? gstNumber)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        GstNumber = gstNumber?.Trim();
        Touch();
    }
}
