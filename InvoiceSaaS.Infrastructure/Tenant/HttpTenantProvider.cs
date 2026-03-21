using Microsoft.AspNetCore.Http;

namespace InvoiceSaaS.Infrastructure.Tenant;

public sealed class HttpTenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    private static readonly Guid DefaultTenant = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid GetTenantId()
    {
        if (httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tenantId && tenantId != Guid.Empty)
        {
            return tenantId;
        }

        return DefaultTenant;
    }
}
