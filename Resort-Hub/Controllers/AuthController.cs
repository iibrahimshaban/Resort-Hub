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
            return View(model);

        var result = await _authService.RegisterAsync(model);

        if (result.IsSuccess)
            return RedirectToAction(nameof(ConfirmEmail), new { email = model.Email });

        TempData.SetError(result.Error);
        return View(model);
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
        return RedirectToAction(nameof(Login));
    }
    [HttpGet]
    public IActionResult ConfirmEmail(string email)
    {
        return View(new ConfirmEmailVM { Email = email });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.ConfirmEmailAsync(model);

        if (result.IsSuccess)
        {
            TempData["ClearHistory"] = true;
            return RedirectToAction(nameof(Login));
        }

        TempData.SetError(result.Error);
        return View(model);
    }

    // ── RESEND CONFIRMATION ──────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> ResendConfirmation(string email,
        CancellationToken cancellationToken)
    {
        await _authService.ReasendEmailConfiramtionCode(email, cancellationToken);
        return RedirectToAction(nameof(ConfirmEmail), new { email });
    }

    // ── FORGOT PASSWORD ──────────────────────────────────────
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _authService.SendResetPasswordCodeAsync(model.Email, cancellationToken);

        return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
    }
    [HttpGet]
    public IActionResult ResetPassword(string email, string otp = null)
    {
        return View(new ResetPasswordVM { Email = email, Otp = otp });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.ResetPasswordAsync(model, cancellationToken);

        if (result.IsSuccess)
        {
            TempData["ClearHistory"] = true;
            return RedirectToAction(nameof(Login));
        }

        TempData.SetError(result.Error);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> VerifyResetOtp(string email, string otp, CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyResetOtpAsync(email, otp, cancellationToken);

        if (result.IsSuccess)
        {
            // Pass email + verified otp to the password step
            return RedirectToAction(nameof(ResetPassword), new { email, otp });
        }

        TempData.SetError(result.Error);
        return RedirectToAction(nameof(ResetPassword), new { email });
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
