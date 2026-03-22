using InvoiceSaaS.Application.DTOs.Company;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[ApiController]
[Route("api/companies")]
public sealed class CompanyController(ICompanyService companyService, ITenantProvider tenantProvider) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyRequest request)
    {
        // Step 12: Company creation requires TenantId (it's in CreateCompanyRequest)
        var result = await companyService.CreateCompanyAsync(request);
        return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
    {
        var result = await companyService.GetCompanyAsync(id);
        return Ok(result);
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompaniesByTenant(Guid tenantId)
    {
        // Step 12: Add endpoint: GET /api/companies/tenant/{tenantId}
        var results = await companyService.GetCompaniesByTenantAsync(tenantId);
        return Ok(results);
    }
}
