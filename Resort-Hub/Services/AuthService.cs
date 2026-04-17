using Microsoft.AspNetCore.Authentication;
using Resort_Hub.Abstraction.Consts;
using Resort_Hub.ViewModels.Auth;
using System.Security.Claims;

namespace Resort_Hub.Services;

public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task<Result> LoginAsync(LoginVM request)
    {
        var current = await _userManager.FindByNameAsync(request.UserName);

        if (current != null)
        {
            bool found = await _userManager.CheckPasswordAsync(current, request.Password);

            if (found)
            {
                await _signInManager.SignInAsync(current, request.RememberMe);
                return Result.Success();
            }
        }

        return Result.Failure(UserError.InvaildCredintial);
    }
    public async Task<Result> RegisterAsync(RegisterVM request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);


        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Customer.Name);
            await _signInManager.SignInAsync(user, isPersistent: true);


            return Result.Success();
        }

        var error = result.Errors.FirstOrDefault();

        if (error is null)
            return Result.Failure(UserError.RegistrationFailed);

        return Result.Failure(new Error(error.Code, error.Description, ErrorType.Failure));
    }
    public async Task<Result> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Result.Success();
    }

    public AuthenticationProperties ExternalLoginAsync(string redirectUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return properties;
    }

    public async Task<Result> ExternalLoggingCallBackAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return Result.Failure(UserError.ExternalLoginFailed);

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (result.Succeeded)
            return Result.Success();

        if (result.IsLockedOut)
            return Result.Failure(UserError.AccountLockedOut);

        if (result.IsNotAllowed)
            return Result.Failure(UserError.EmailNotConfirmed);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var picture = info.Principal.FindFirstValue("picture");

        if (string.IsNullOrEmpty(email))
            return Result.Failure(UserError.ExternalLoginFailed);

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            return Result.Success();
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureUrl = picture,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            return Result.Failure(UserError.RegistrationFailed);

        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return Result.Success();
    }
}
