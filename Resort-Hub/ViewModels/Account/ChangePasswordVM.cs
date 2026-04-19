using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Account;

public class ChangePasswordVM
{
    [Required(ErrorMessage ="*")]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required(ErrorMessage = "*")]
    public string NewPassword { get; set; } = string.Empty;
}
