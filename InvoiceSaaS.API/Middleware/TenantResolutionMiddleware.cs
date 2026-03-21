namespace InvoiceSaaS.API.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Tenant-Id";
    private static readonly Guid FallbackTenant = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantHeader = context.Request.Headers[HeaderName].FirstOrDefault();
        var resolvedTenant = Guid.TryParse(tenantHeader, out var tenantId) && tenantId != Guid.Empty
            ? tenantId
            : FallbackTenant;

        context.Items["TenantId"] = resolvedTenant;
        await next(context);
    }
}
