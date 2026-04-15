using Resort_Hub.Abstraction.Consts;
using Resort_Hub.ViewModels.Auth;

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
}
