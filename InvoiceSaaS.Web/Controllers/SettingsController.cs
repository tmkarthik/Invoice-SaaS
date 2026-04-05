using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public sealed class SettingsController(
    ISettingsService settingsService,
    ITaxService taxService,
    ITenantProvider tenantProvider) : Controller
{
    public async Task<IActionResult> Index()
    {
        var tenantId = tenantProvider.GetTenantId();
        var settings = await settingsService.GetSettingsAsync(tenantId);
        var taxes = await taxService.GetTaxesAsync(tenantId);

        return View(new SettingsViewModel
        {
            InvoiceSettings = settings,
            Taxes = taxes.ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateSettingsDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        try
        {
            await settingsService.UpdateSettingsAsync(tenantId, dto);
            TempData["SuccessMessage"] = "General settings updated.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Update failed: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTax(CreateTaxDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        try
        {
            await taxService.CreateTaxAsync(tenantId, dto);
            TempData["SuccessMessage"] = "Tax definition added.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to add tax: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTax(Guid id)
    {
        var tenantId = tenantProvider.GetTenantId();
        try
        {
            await taxService.DeleteTaxAsync(tenantId, id);
            TempData["SuccessMessage"] = "Tax definition removed.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to remove tax: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
