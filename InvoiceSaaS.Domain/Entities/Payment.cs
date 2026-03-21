using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public Payment(Guid tenantId, Guid invoiceId, decimal amount, PaymentMethod method)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (invoiceId == Guid.Empty) throw new ArgumentException("InvoiceId is required.", nameof(invoiceId));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        SetTenant(tenantId);
        InvoiceId = invoiceId;
        Amount = amount;
        PaymentDate = DateTime.UtcNow;
        PaymentMethod = method;
    }

    private Payment() { }

    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? TransactionId { get; private set; }
    public string? Notes { get; private set; }

    public void ApplyPayment(string? transactionId, string? notes)
    {
        TransactionId = transactionId?.Trim();
        Notes = notes?.Trim();
        Touch();
    }
}
