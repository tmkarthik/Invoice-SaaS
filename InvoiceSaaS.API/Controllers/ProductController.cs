using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.DTOs.Product;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ProductController(IProductService productService, ITenantProvider tenantProvider) : ControllerBase
{
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

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetByCompany(Guid companyId)
    {
        var products = await productService.GetByCompanyAsync(companyId);
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
        var product = await productService.CreateAsync(dto);
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
