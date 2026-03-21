using Microsoft.AspNetCore.Http;
using InvoiceSaaS.Application.Interfaces;

namespace InvoiceSaaS.Infrastructure.Tenant;

public sealed class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    private Guid? _tenantId;

    public Guid GetTenantId()
    {
        if (_tenantId.HasValue) return _tenantId.Value;

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new UnauthorizedAccessException("Tenant ID is missing. Context is null.");

        // Check JWT Claim
        var tenantClaim = httpContext.User?.FindFirst("TenantId")?.Value;
        if (Guid.TryParse(tenantClaim, out var tenantIdFromClaim) && tenantIdFromClaim != Guid.Empty)
        {
            _tenantId = tenantIdFromClaim;
            return _tenantId.Value;
        }

        // Check HTTP Header Fallback
        var tenantHeader = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(tenantHeader, out var tenantIdFromHeader) && tenantIdFromHeader != Guid.Empty)
        {
            _tenantId = tenantIdFromHeader;
            return _tenantId.Value;
        }

        throw new UnauthorizedAccessException("Tenant ID is missing.");
    }

    public void SetTenantId(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));

        _tenantId = tenantId;
    }

    public bool IsAdmin => httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
}
