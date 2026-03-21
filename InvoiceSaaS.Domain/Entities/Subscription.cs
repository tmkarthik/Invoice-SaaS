using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Subscription : BaseEntity
{
    public Subscription(Guid tenantId, string planName, DateTime startDate, DateTime endDate, int maxUsers, int maxInvoices)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(planName)) throw new ArgumentException("Plan name is required.", nameof(planName));

        SetTenant(tenantId);
        PlanName = planName.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        MaxUsers = maxUsers;
        MaxInvoices = maxInvoices;
    }

    private Subscription() { }

    public string PlanName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxUsers { get; private set; }
    public int MaxInvoices { get; private set; }

    public bool IsExpired() => DateTime.UtcNow > EndDate;

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
