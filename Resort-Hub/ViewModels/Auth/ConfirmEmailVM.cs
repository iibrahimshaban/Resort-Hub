using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class ConfirmEmailVM
{
    [Required(ErrorMessage ="*"),EmailAddress(ErrorMessage ="Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage ="*")]
    public string Code { get; set; } = string.Empty;
}
