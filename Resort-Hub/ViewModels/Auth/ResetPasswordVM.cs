using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class ResetPasswordVM
{
    [Required(ErrorMessage = "*"), EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "*")]
    [DataType(DataType.Password), Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password), Display(Name = "Confirm Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Password doesn't match confirm password ")]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "*")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 characters long")]
    public string Otp { get; set; } = string.Empty;
}
