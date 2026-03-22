using InvoiceSaaS.Application.DTOs.Tenant;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantController(ITenantService tenantService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateTenant(CreateTenantRequest request)
    {
        // Step 16: Implement onboarding flow via RegisterTenantAsync
        var result = await tenantService.RegisterTenantAsync(request);
        return CreatedAtAction(nameof(GetTenant), new { id = result.Tenant.Id }, ApiResponse.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var result = await tenantService.GetTenantAsync(id);
        return Ok(ApiResponse.SuccessResponse(result));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTenants()
    {
        // Security: This should be admin-only. The [Authorize] policy "Admin" can be used here.
        var result = await tenantService.GetAllTenantsAsync();
        return Ok(ApiResponse.SuccessResponse(result)); 
    }

    [HttpPut("{id}/upgrade")]
    public async Task<IActionResult> UpgradeTenant(Guid id, UpgradeTenantRequest request)
    {
        await tenantService.UpgradeTenantAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Tenant upgraded successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateTenant(Guid id)
    {
        await tenantService.DeactivateTenantAsync(id);
        return Ok(ApiResponse.SuccessResponse("Tenant deactivated successfully."));
    }
}
