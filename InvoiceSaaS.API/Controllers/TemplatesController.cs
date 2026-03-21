using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.DTOs.Template;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TemplatesController(ITemplateService templateService) : ControllerBase
{
    private Guid GetCompanyId()
    {
        var tenantClaim = User.FindFirst("TenantId")?.Value;
        return Guid.TryParse(tenantClaim, out var tenantId) ? tenantId : Guid.Empty;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var template = await templateService.GetByIdAsync(id);
        return template != null 
            ? Ok(ApiResponse.SuccessResponse(template)) 
            : NotFound(ApiResponse.Failure("Template not found."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var templates = await templateService.GetAllAsync();
        return Ok(ApiResponse.SuccessResponse(templates));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTemplateDto dto)
    {
        var companyId = GetCompanyId();
        if (companyId == Guid.Empty) return Unauthorized(ApiResponse.Failure("Invalid Tenant."));

        var template = await templateService.CreateAsync(companyId, dto);
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, ApiResponse.SuccessResponse(template, "Template created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTemplateDto dto)
    {
        await templateService.UpdateAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse("Template updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await templateService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse("Template deleted successfully."));
    }
}
