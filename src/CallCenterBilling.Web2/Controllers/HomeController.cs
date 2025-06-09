using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CallCenterBilling.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}