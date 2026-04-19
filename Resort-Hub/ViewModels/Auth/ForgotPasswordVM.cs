using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class ForgotPasswordVM
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
}
