namespace InvoiceSaaS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        }

        TenantId = tenantId;
        Touch();
    }

    public void Touch()
    {
        UpdatedDate = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Touch();
    }

    public void Restore()
    {
        IsDeleted = false;
        Touch();
    }
}
