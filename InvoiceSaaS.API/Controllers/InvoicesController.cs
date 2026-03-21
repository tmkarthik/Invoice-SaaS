using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class InvoicesController(IInvoiceService invoiceService, ITenantProvider tenantProvider) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.GetTenantId();
        var invoices = await invoiceService.GetInvoicesAsync(tenantId, cancellationToken);
        return Ok(ApiResponse.SuccessResponse(invoices));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.GetTenantId();
        if (tenantId == Guid.Empty) return Unauthorized(ApiResponse.Failure("Invalid Tenant."));

        var invoice = await invoiceService.CreateInvoiceAsync(tenantId, dto, cancellationToken);
        return Created($"/api/invoices/{invoice.Id}", ApiResponse.SuccessResponse(invoice, "Invoice created successfully."));
    }
}
