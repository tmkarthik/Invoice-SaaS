using InvoiceSaaS.Application.DTOs.Company;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CompanyController(ICompanyService companyService, ITenantProvider tenantProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCompany(CreateCompanyRequest request)
    {
        var result = await companyService.CreateCompanyAsync(request);
        return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, ApiResponse.SuccessResponse(result, "Company created successfully."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var result = await companyService.GetCompanyAsync(id);
        return Ok(ApiResponse.SuccessResponse(result));
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<IActionResult> GetCompaniesByTenant(Guid tenantId)
    {
        var results = await companyService.GetCompaniesByTenantAsync(tenantId);
        return Ok(ApiResponse.SuccessResponse(results));
    }
}
