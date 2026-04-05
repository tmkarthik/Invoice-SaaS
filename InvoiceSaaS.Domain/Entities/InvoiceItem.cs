using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class InvoiceItem : BaseEntity
{
    public InvoiceItem(Guid invoiceId, Guid productId, string description, decimal quantity, decimal unitPrice)
    {
        if (invoiceId == Guid.Empty) throw new ArgumentException("InvoiceId cannot be empty.", nameof(invoiceId));
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        if (unitPrice <= 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");

        InvoiceId = invoiceId;
        ProductId = productId;
        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
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

    private readonly List<InvoiceItemTax> _taxes = [];
    public IReadOnlyCollection<InvoiceItemTax> Taxes => _taxes.AsReadOnly();

    public void AddTax(TaxDefinition tax)
    {
        ArgumentNullException.ThrowIfNull(tax);
        var itemTax = new InvoiceItemTax(Id, tax.Id, tax.Name, tax.Rate, tax.IsCompound, tax.Priority);
        if (TenantId != Guid.Empty) itemTax.SetTenant(TenantId);
        _taxes.Add(itemTax);
    }

    public decimal GetLineTotal()
    {
        var subtotal = Quantity * UnitPrice;
        var totalTax = 0m;
        var currentTaxableAmount = subtotal;

        foreach (var tax in _taxes.OrderBy(x => x.Priority))
        {
            var taxAmount = currentTaxableAmount * tax.AppliedRate;
            totalTax += taxAmount;
            if (tax.IsCompound)
            {
                currentTaxableAmount += taxAmount;
            }
            tax.SetAmount(taxAmount);
        }

        return subtotal + totalTax;
    }
}
