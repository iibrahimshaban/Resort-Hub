using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class LoginVM
{
    [Display(Name = "Email"), Required(ErrorMessage = "*")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [DataType(DataType.Password), Required(ErrorMessage = "*")]
    public string Password { get; set; }
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
