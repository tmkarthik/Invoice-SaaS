using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Address : BaseEntity
{
    private readonly List<CustomerAddress> _customerAddresses = [];

    public Address(Guid tenantId, string addressLine1, string city, string country, string postalCode, string? addressLine2 = null, string? state = null, string? landmark = null)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(addressLine1)) throw new ArgumentException("Address line 1 is required.", nameof(addressLine1));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.", nameof(country));

        SetTenant(tenantId);
        AddressLine1 = addressLine1.Trim();
        AddressLine2 = addressLine2?.Trim();
        City = city.Trim();
        State = state?.Trim();
        Country = country.Trim();
        PostalCode = postalCode.Trim();
        Landmark = landmark?.Trim();
    }

    private Address() { }

    public string AddressLine1 { get; private set; } = string.Empty;
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string? State { get; private set; }
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string? Landmark { get; private set; }

    public IReadOnlyCollection<CustomerAddress> CustomerAddresses => _customerAddresses.AsReadOnly();

    public void UpdateAddress(string addressLine1, string city, string country, string postalCode, string? addressLine2 = null, string? state = null, string? landmark = null)
    {
        if (string.IsNullOrWhiteSpace(addressLine1)) throw new ArgumentException("Address line 1 is required.", nameof(addressLine1));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.", nameof(country));

        AddressLine1 = addressLine1.Trim();
        AddressLine2 = addressLine2?.Trim();
        City = city.Trim();
        State = state?.Trim();
        Country = country.Trim();
        PostalCode = postalCode.Trim();
        Landmark = landmark?.Trim();
        Touch();
    }
}
