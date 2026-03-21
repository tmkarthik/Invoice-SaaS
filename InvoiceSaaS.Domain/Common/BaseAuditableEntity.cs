namespace InvoiceSaaS.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    public void MarkCreated(string? user)
    {
        CreatedBy = user;
        UpdatedBy = user;
        Touch();
    }

    public void MarkUpdated(string? user)
    {
        UpdatedBy = user;
        Touch();
    }
}
