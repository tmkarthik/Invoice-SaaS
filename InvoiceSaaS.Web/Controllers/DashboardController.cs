using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public sealed class DashboardController(
    IDashboardService dashboardService,
    ITenantProvider tenantProvider) : Controller
{
    public async Task<IActionResult> Index()
    {
        var tenantId = tenantProvider.GetTenantId();
        if (tenantId == Guid.Empty) return RedirectToAction("Login", "Auth");

        var summary = await dashboardService.GetDashboardSummaryAsync(tenantId);
        return View(summary);
    }
}
