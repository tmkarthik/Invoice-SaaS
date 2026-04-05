using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

[Route("portal")]
public sealed class PortalController(
    IInvoiceService invoiceService,
    IPaymentService paymentService,
    IPdfService pdfService) : Controller
{
    [HttpGet("{customerId}")]
    public async Task<IActionResult> Index(Guid customerId)
    {
        // Future: Check if customer has a valid magic link or session
        var invoices = await invoiceService.GetInvoicesAsync(); // Future: Filter by customerId
        var customerInvoices = invoices.Where(i => i.CustomerId == customerId);
        
        ViewBag.CustomerId = customerId;
        return View(customerInvoices);
    }

    [HttpGet("invoice/{id}")]
    public async Task<IActionResult> ViewInvoice(Guid id)
    {
        var invoice = await invoiceService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();
        
        return View(invoice);
    }

    [HttpPost("invoice/{id}/pay")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var invoice = await invoiceService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();

        var successUrl = $"{Request.Scheme}://{Request.Host}/portal/invoice/{id}/success";
        var cancelUrl = $"{Request.Scheme}://{Request.Host}/portal/invoice/{id}/cancel";

        var sessionUrl = await paymentService.CreateCheckoutSessionAsync(
            id,
            invoice.Amount, 
            invoice.Currency, 
            successUrl, 
            cancelUrl, 
            invoice.CustomerName ?? "");

        return Redirect(sessionUrl);
    }

    [HttpGet("invoice/{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var invoice = await invoiceService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();

        // This would render the Editor.cshtml view to HTML and then convert to PDF
        // For now, a placeholder HTML string
        var html = $"<h1>Invoice {invoice.InvoiceNumber}</h1><p>Amount: {invoice.Amount} {invoice.Currency}</p>";
        var pdf = await pdfService.GeneratePdfFromHtmlAsync(html);
        
        return File(pdf, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
    }
}
