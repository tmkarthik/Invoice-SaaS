using InvoiceSaaS.Application.DTOs.Template;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class TemplateBuilderController(ITemplateService templateService) : Controller
{
    // Screen SPA mapped to Editor logic
    public IActionResult Editor(Guid? id)
    {
        // For existing templates, you would fetch it here:
        // var template = _templateService.GetByIdAsync(id.Value);
        
        var vm = new TemplateDataViewModel
        {
            Company = new CompanyInfoViewModel { Name = "Acme Design Studio", Address = "123 Creative Ave", Email = "hello@acmestudio.com" },
            Customer = new CustomerInfoViewModel { Name = "TechFlow Inc.", ContactPerson = "Mark Johnson", BillingAddress = "456 Startup Blvd" },
            InvoiceDetails = new InvoiceDetailsViewModel { InvoiceNumber = "INV-2024-0042", IssueDate = "2024-04-01", DueDate = "2024-04-30" }
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] CreateTemplateDto dto)
    {
        try
        {
            // Usually fetched from session/claims:
            var companyId = Guid.NewGuid(); 
            var result = await templateService.CreateAsync(companyId, dto);
            return Json(new { success = true, templateId = result.Id, message = "Saved successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
