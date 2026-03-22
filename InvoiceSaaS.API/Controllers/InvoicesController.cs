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
        var invoices = await invoiceService.GetInvoicesAsync(cancellationToken);
        return Ok(ApiResponse.SuccessResponse(invoices));
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetByCompany(Guid companyId, CancellationToken cancellationToken)
    {
        var invoices = await invoiceService.GetInvoicesByCompanyAsync(companyId, cancellationToken);
        return Ok(ApiResponse.SuccessResponse(invoices));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var invoice = await invoiceService.CreateInvoiceAsync(dto, cancellationToken);
        return Created($"/api/invoices/{invoice.Id}", ApiResponse.SuccessResponse(invoice, "Invoice created successfully."));
    }
}
