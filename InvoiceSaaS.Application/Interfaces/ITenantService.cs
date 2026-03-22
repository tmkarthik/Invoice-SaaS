using InvoiceSaaS.Application.DTOs.Tenant;

namespace InvoiceSaaS.Application.Interfaces;

public sealed record RegisterTenantResponse(TenantDto Tenant, AuthResult Auth);

public interface ITenantService
{
    Task<TenantDto> CreateTenantAsync(CreateTenantRequest request);
    Task<TenantDto> GetTenantAsync(Guid tenantId);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
    Task UpgradeTenantAsync(Guid tenantId, UpgradeTenantRequest request);
    Task DeactivateTenantAsync(Guid tenantId);
    Task<RegisterTenantResponse> RegisterTenantAsync(CreateTenantRequest request); // Adding this for Step 16 onboarding flow
}
