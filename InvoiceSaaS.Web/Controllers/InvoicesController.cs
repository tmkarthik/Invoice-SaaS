using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class InvoicesController(IInvoiceService invoiceService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var invoices = await invoiceService.GetInvoicesAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var invoice = await invoiceService.GetInvoiceByIdAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }
        return View(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> BulkAction(IEnumerable<Guid> selectedIds, string action)
    {
        if (selectedIds == null || !selectedIds.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        switch (action)
        {
            case "MarkAsPaid":
                await invoiceService.BulkMarkAsPaidAsync(selectedIds);
                TempData["SuccessMessage"] = $"{selectedIds.Count()} invoices marked as paid.";
                break;
            case "Delete":
                await invoiceService.BulkDeleteAsync(selectedIds);
                TempData["ErrorMessage"] = $"{selectedIds.Count()} invoices deleted.";
                break;
        }

        return RedirectToAction(nameof(Index));
    }
}
