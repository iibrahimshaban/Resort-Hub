using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Auth;
using System.Security.Claims;

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
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Login", "Auth");
    }
    [HttpGet]
    public async Task<IActionResult> ExternalLogin(string provider = "Google")
    {
        var properties = _authService.ExternalLoginAsync(Url.Action(nameof(ExternalLoginCallback)));
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var result = await _authService.ExternalLoggingCallBackAsync();
        if (result.IsSuccess)
        {
            return RedirectToAction("index", "Home");
        }
        else
        {
            TempData.SetError(result.Error);
            return RedirectToAction(nameof(Login));
        }
    }
}
