using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class InvoiceItem : BaseEntity
{
    public InvoiceItem(Guid invoiceId, Guid productId, string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        if (invoiceId == Guid.Empty) throw new ArgumentException("InvoiceId cannot be empty.", nameof(invoiceId));
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        if (taxRate < 0) throw new ArgumentOutOfRangeException(nameof(taxRate), "Tax rate cannot be negative.");

        InvoiceId = invoiceId;
        ProductId = productId;
        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }

    private InvoiceItem()
    {
    }

    public Guid InvoiceId { get; private set; }
    public Invoice? Invoice { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }

    public decimal GetLineTotal()
    {
        var subtotal = Quantity * UnitPrice;
        return subtotal + (subtotal * TaxRate);
    }
}
