using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class TemplatesController(ITemplateService templateService) : Controller
{
    // Screen 5: Templates Gallery
    public async Task<IActionResult> Index()
    {
        var templates = await templateService.GetAllAsync();
        
        var vm = new TemplateListViewModel
        {
            Templates = templates.Select(t => new TemplateSummaryViewModel
            {
                // Mapping the real DB values to the view model
                Name = t.Name,
                IsDefault = t.IsDefault,
                ColorVariant = "indigo", // Or parse from JSON if it was stored
                MetaLabel = t.IsDefault ? "Default" : "Custom"
            }).ToList()
        };

        // If no templates, supply some defaults or let view handle the "Blank" template fallback
        return View(vm);
    }
}
