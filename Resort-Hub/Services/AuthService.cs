using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Resort_Hub.Abstraction.Consts;
using Resort_Hub.Helpers;
using Resort_Hub.Persistence;
using Resort_Hub.ViewModels.Auth;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Resort_Hub.Services;

public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
    , ICustomEmailService emailService, ApplicationDbContext context) : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ICustomEmailService _emailService = emailService;
    private readonly ApplicationDbContext _context = context;

    private const int OtpExpiryIn = 15;
    public async Task<Result> LoginAsync(LoginVM request)
    {
        var current = await _userManager.FindByEmailAsync(request.Email);

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

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return error is null
                ? Result.Failure(UserError.RegistrationFailed)
                : Result.Failure(new Error(error.Code, error.Description, ErrorType.Failure));
        }

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        await _context.OtpEntries.AddAsync(new OtpEntry
        {
            Code = code,
            Email = user.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryIn),
            Purpose = OtpPurpose.EmailConfirmation
        });

        await _context.SaveChangesAsync();

        var html = EmailBodyBuilder.EmailConfirmation(user.FirstName, code);

        BackgroundJob.Enqueue(() =>
         _emailService.SendEmailAsync(user.Email, "Verify your ResortHub email", html)
        );

        return Result.Success();
    }
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailVM request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        if (user.EmailConfirmed)
            return Result.Failure(UserError.UserAlreadyExists);

        var otp = await _context.OtpEntries
            .Where(x => x.Email == user.Email
                     && x.Purpose == OtpPurpose.EmailConfirmation
                     && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null || otp.IsExpired)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        if (request.Code != otp.Code)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        otp.IsUsed = true;
        await _context.SaveChangesAsync();


        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        await _userManager.AddToRoleAsync(user, DefaultRoles.Customer.Name);
        await _signInManager.SignInAsync(user, isPersistent: true);

        return Result.Success();
    }

    public async Task<Result> ReasendEmailConfiramtionCode(string email, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserError.DuplicatedConfirmation);


        var newOtp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        await _context.OtpEntries
            .Where(x => x.Email == email && x.Purpose == OtpPurpose.EmailConfirmation && !x.IsUsed)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(x => x.IsUsed, true), cancellationToken);

        await _context.OtpEntries.AddAsync(new OtpEntry
        {
            Code = newOtp,
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryIn),
            Purpose = OtpPurpose.EmailConfirmation
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var html = EmailBodyBuilder.EmailConfirmation(user.FirstName, newOtp);

        BackgroundJob.Enqueue(() =>
         _emailService.SendEmailAsync(user.Email, "Verify your ResortHub email", html)
        );

        return Result.Success();
    }
    public async Task<Result> SendResetPasswordCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var otpEntry = new OtpEntry
        {
            Email = email,
            Code = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryIn),
            Purpose = OtpPurpose.PasswordReset
        };

        await _context.AddAsync(otpEntry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var html = EmailBodyBuilder.PasswordReset(user.FirstName, otpCode);
        BackgroundJob.Enqueue(() =>
            _emailService.SendEmailAsync(user.Email, "Reset your ResortHub password", html)
        );

        return Result.Success();
    }
    public async Task<Result> ResetPasswordAsync(ResetPasswordVM request,CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        var otp = await _context.OtpEntries
            .Where(x => x.Email == request.Email
                     && x.Purpose == OtpPurpose.PasswordReset
                     && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null || otp.IsExpired)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        if (otp.Code != request.Otp)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!resetResult.Succeeded)
        {
            var error = resetResult.Errors.FirstOrDefault();
            return error is null
                ? Result.Failure(UserError.RegistrationFailed)
                : Result.Failure(new Error(error.Code, error.Description, ErrorType.Failure));
        }

        otp.IsUsed = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> VerifyResetOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(otp) || otp.Length != 6)
            return Result.Failure(UserError.OtpInvalidOrExpired);

        var otpEntry = await _context.OtpEntries
            .Where(x => x.Email == email
                     && x.Purpose == OtpPurpose.PasswordReset
                     && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpEntry is not null && !otpEntry.IsExpired && otpEntry.Code == otp)
            return Result.Success();

        return Result.Failure(UserError.OtpInvalidOrExpired);
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
