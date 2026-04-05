using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class InvoicesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
