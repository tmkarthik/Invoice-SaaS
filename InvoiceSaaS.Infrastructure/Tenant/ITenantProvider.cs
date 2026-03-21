namespace InvoiceSaaS.Infrastructure.Tenant;

public interface ITenantProvider
{
    Guid GetTenantId();
}
