using Microsoft.AspNetCore.Authentication;
using Resort_Hub.ViewModels.Auth;

namespace Resort_Hub.Services;

public interface IAuthService
{
    Task<Result> LoginAsync(LoginVM request);
    Task<Result> RegisterAsync(RegisterVM request);
    Task<Result> ConfirmEmailAsync(ConfirmEmailVM request);
    Task<Result> ReasendEmailConfiramtionCode(string email, CancellationToken cancellationToken = default);
    Task<Result> SendResetPasswordCodeAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordVM request, CancellationToken cancellationToken = default);
    Task<Result> VerifyResetOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync();
    AuthenticationProperties ExternalLoginAsync(string redirectUrl);
    Task<Result> ExternalLoggingCallBackAsync();
}
