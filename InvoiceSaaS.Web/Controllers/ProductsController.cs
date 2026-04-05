using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.Web.Controllers;

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
