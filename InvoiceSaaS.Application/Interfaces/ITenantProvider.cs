namespace InvoiceSaaS.Application.Interfaces;

public interface ITenantProvider
{
    Guid GetTenantId();
    void SetTenantId(Guid tenantId);
    bool IsAdmin { get; }
}
