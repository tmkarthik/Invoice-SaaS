using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InvoiceSaaS.API.Middleware;

public sealed class TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        // 1. Check JWT Claims
        var tenantClaim = context.User?.FindFirst("TenantId")?.Value;
        if (Guid.TryParse(tenantClaim, out var tenantId) && tenantId != Guid.Empty)
        {
            tenantProvider.SetTenantId(tenantId);
            await next(context);
            return;
        }

        // 2. Check HTTP Header
        var tenantHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(tenantHeader, out tenantId) && tenantId != Guid.Empty)
        {
            tenantProvider.SetTenantId(tenantId);
            await next(context);
            return;
        }

        // 3. Missing Tenant handling
        logger.LogWarning("Tenant ID is missing in both JWT claim and X-Tenant-Id header. Pipeline short-circuited.");
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Tenant ID is missing.");
    }
}
