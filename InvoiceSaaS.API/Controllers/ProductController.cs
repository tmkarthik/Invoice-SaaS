using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.DTOs.Product;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ProductController(IProductService productService) : ControllerBase
{
    private Guid GetCompanyId()
    {
        var tenantClaim = User.FindFirst("TenantId")?.Value;
        return Guid.TryParse(tenantClaim, out var tenantId) ? tenantId : Guid.Empty;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await productService.GetByIdAsync(id);
        return product != null 
            ? Ok(ApiResponse.SuccessResponse(product)) 
            : NotFound(ApiResponse.Failure("Product not found."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllAsync();
        return Ok(ApiResponse.SuccessResponse(products));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var (items, totalCount) = await productService.GetPagedAsync(page, pageSize, search);
        return Ok(ApiResponse.SuccessResponse(new { items, totalCount, page, pageSize }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var companyId = GetCompanyId();
        if (companyId == Guid.Empty) return Unauthorized(ApiResponse.Failure("Invalid Tenant."));

        var product = await productService.CreateAsync(companyId, dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ApiResponse.SuccessResponse(product, "Product created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        await productService.UpdateAsync(id, dto);
        return Ok(ApiResponse.SuccessResponse("Product updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await productService.DeleteAsync(id);
        return Ok(ApiResponse.SuccessResponse("Product deleted successfully."));
    }
}
