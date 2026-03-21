using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InvoicesController(IInvoiceService invoiceService, ITenantProvider tenantProvider) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.GetTenantId();
        var invoices = await invoiceService.GetInvoicesAsync(tenantId, cancellationToken);
        return Ok(invoices);
    }
}
