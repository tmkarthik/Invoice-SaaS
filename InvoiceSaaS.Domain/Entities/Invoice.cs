using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Invoice : BaseAuditableEntity
{
    private readonly List<InvoiceItem> _invoiceItems = [];

    public Invoice(Guid tenantId, Guid companyId, Guid customerId, string number, DateTime issueDateUtc, DateTime dueDateUtc, string currency)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (customerId == Guid.Empty) throw new ArgumentException("CustomerId cannot be empty.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Invoice number is required.", nameof(number));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));

        SetTenant(tenantId);
        CompanyId = companyId;
        CustomerId = customerId;
        Number = number.Trim();
        IssueDateUtc = issueDateUtc;
        DueDateUtc = dueDateUtc;
        Currency = currency.Trim().ToUpperInvariant();
    }

    private Invoice()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public Tenant? Tenant { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateTime IssueDateUtc { get; private set; }
    public DateTime DueDateUtc { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal Subtotal { get; private set; }
    public decimal TotalTax { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Amount { get; private set; }
    public string? Notes { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    private readonly List<Payment> _payments = [];
    public IReadOnlyCollection<InvoiceItem> InvoiceItems => _invoiceItems.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public void SetStatus(InvoiceStatus status)
    {
        Status = status;
        Touch();
    }

    public void SetDiscount(decimal discount)
    {
        if (discount < 0) throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot be negative.");
        Discount = discount;
        RecalculateAmount();
        Touch();
    }

    public void AddItem(InvoiceItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.InvoiceId != Id)
        {
            throw new InvalidOperationException("Invoice item does not belong to this invoice.");
        }

        if (TenantId != Guid.Empty)
        {
            item.SetTenant(TenantId);
        }

        _invoiceItems.Add(item);
        RecalculateAmount();
        Touch();
    }

    public void SetNotes(string? notes)
    {
        Notes = notes?.Trim();
        Touch();
    }

    private void RecalculateAmount()
    {
        Subtotal = _invoiceItems.Sum(x => x.Quantity * x.UnitPrice);
        TotalTax = _invoiceItems.Sum(x => x.GetLineTotal() - (x.Quantity * x.UnitPrice));
        
        var calculatedTotal = Subtotal + TotalTax - Discount;
        Amount = calculatedTotal < 0 ? 0 : calculatedTotal;
    }
}
