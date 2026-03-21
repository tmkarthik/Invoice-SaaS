using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Domain.Entities;

public sealed class CustomerAddress : BaseEntity
{
    public CustomerAddress(Guid tenantId, Guid customerId, Guid addressId, AddressType type, bool isDefault = false)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (customerId == Guid.Empty) throw new ArgumentException("CustomerId is required.", nameof(customerId));
        if (addressId == Guid.Empty) throw new ArgumentException("AddressId is required.", nameof(addressId));

        SetTenant(tenantId);
        CustomerId = customerId;
        AddressId = addressId;
        AddressType = type;
        IsDefault = isDefault;
    }

    private CustomerAddress() { }

    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public Guid AddressId { get; private set; }
    public Address? Address { get; private set; }
    public AddressType AddressType { get; private set; }
    public bool IsDefault { get; private set; }

    public void SetDefault()
    {
        IsDefault = true;
        Touch();
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        Touch();
    }
}
