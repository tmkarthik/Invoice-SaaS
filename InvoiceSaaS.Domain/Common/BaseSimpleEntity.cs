namespace InvoiceSaaS.Domain.Common;

public abstract class BaseSimpleEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; protected set; } = DateTime.UtcNow;
    public bool IsDeleted { get; protected set; }

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
