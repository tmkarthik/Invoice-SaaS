using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
