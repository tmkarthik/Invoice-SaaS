using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class CustomersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
