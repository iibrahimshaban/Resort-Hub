using Microsoft.AspNetCore.Authentication;
using Resort_Hub.ViewModels.Auth;

namespace Resort_Hub.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterVM request);
    Task<Result> LoginAsync(LoginVM request);
    Task<Result> LogoutAsync();
    AuthenticationProperties ExternalLoginAsync(string redirectUrl);
    Task<Result> ExternalLoggingCallBackAsync();
}
