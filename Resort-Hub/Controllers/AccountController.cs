using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Account;
using System.Security.Claims;

namespace Resort_Hub.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    private readonly IAccountService _accountService = accountService;

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _accountService.GetProfileAsync(userId);
        return PartialView("_ProfileModal", result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileVM request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _accountService.UpdateProfileAsync(userId, request);
        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile Picture)
    {
        if (Picture == null || Picture.Length == 0)
        {
            TempData["Error"] = "Please select an image.";
            return RedirectToAction("Index", "Home");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _accountService.UpdateProfilePictureAsync(userId, Picture);

        if (!result.IsSuccess)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _accountService.DeleteProfilePictureAsync(userId);

        if (!result.IsSuccess)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction("Index", "Home");
    }
}
