using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class InvoiceItemTax : BaseEntity
{
    public InvoiceItemTax(Guid invoiceItemId, Guid taxDefinitionId, string taxName, decimal rate, bool isCompound, int priority)
    {
        InvoiceItemId = invoiceItemId;
        TaxDefinitionId = taxDefinitionId;
        TaxName = taxName;
        AppliedRate = rate;
        IsCompound = isCompound;
        Priority = priority;
    }

    private InvoiceItemTax() { }

    public Guid InvoiceItemId { get; private set; }
    public InvoiceItem? InvoiceItem { get; private set; }

    public Guid TaxDefinitionId { get; private set; }
    public TaxDefinition? TaxDefinition { get; private set; }

    public string TaxName { get; private set; } = string.Empty;
    public decimal AppliedRate { get; private set; }
    public bool IsCompound { get; private set; }
    public int Priority { get; private set; }

    // Calculated for snapshot purposes
    public decimal Amount { get; private set; }

    public void SetAmount(decimal amount)
    {
        Amount = amount;
    }
}
