using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.DTOs.Customer;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CustomerController(ICustomerService customerService, ITenantProvider tenantProvider) : ControllerBase
{
    private Guid GetCompanyId()
    {
        var tenantClaim = User.FindFirst("TenantId")?.Value;
        return Guid.TryParse(tenantClaim, out var tenantId) ? tenantId : Guid.Empty;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await customerService.GetByIdAsync(id);
        return customer != null 
            ? Ok(ApiResponse.SuccessResponse(customer)) 
            : NotFound(ApiResponse.Failure("Customer not found."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await customerService.GetAllAsync();
        return Ok(ApiResponse.SuccessResponse(customers));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var (items, totalCount) = await customerService.GetPagedAsync(page, pageSize, search);
        return Ok(ApiResponse.SuccessResponse(new { items, totalCount, page, pageSize }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var companyId = GetCompanyId();
        if (companyId == Guid.Empty) return Unauthorized(ApiResponse.Failure("Invalid Tenant."));

        var customer = await customerService.CreateAsync(companyId, dto);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, ApiResponse.SuccessResponse(customer, "Customer created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        await customerService.UpdateAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse("Customer updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await customerService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse("Customer deleted successfully."));
    }
}
