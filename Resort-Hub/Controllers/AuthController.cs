using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Auth;

namespace Resort_Hub.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =  await _authService.RegisterAsync(model);
        if (result.IsSuccess)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData.SetError(result.Error);
            return View(model);
        }

    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await _authService.LoginAsync(model);
        if (result.IsSuccess)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData.SetError(result.Error);
            return View(model);
        }
    }
}
