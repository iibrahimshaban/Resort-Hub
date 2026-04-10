using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Abstraction;
using Resort_Hub.Abstraction.Enums;
using Resort_Hub.Models;
using System.Diagnostics;
using Resort_Hub.Handlers.ErrorHandler;
using Resort_Hub.Errors;

namespace Resort_Hub.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Test()
    {
        var result = Result.Failure(TestError.NotFound);
        TempData.SetError(result.Error);
        return View("index");
    }
}
