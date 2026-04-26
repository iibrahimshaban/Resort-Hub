
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Abstraction.Consts;
using Resort_Hub.Models;
using Resort_Hub.Services;
using System.Diagnostics;


namespace Resort_Hub.Controllers;

public class HomeController(IVillaService villaService) : Controller
{
    private readonly IVillaService _villaService = villaService;

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
    public async Task<IActionResult> Test()
    {
        var result = await _villaService.ValidateVilla(100);

        if (!result.IsSuccess)
            TempData.SetError(result.Error);

        return View("index");
    }
}
