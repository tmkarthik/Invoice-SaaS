using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class ActivityLog : BaseEntity
{
    public Guid? InvoiceId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Details { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private ActivityLog() { } // EF Core

    public ActivityLog(Guid tenantId, string action, string details, Guid? invoiceId = null)
    {
        SetTenant(tenantId);
        Action = action;
        Details = details;
        InvoiceId = invoiceId;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
