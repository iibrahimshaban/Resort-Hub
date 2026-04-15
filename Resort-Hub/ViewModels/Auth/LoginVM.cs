using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class LoginVM
{
    [Display(Name = "User Name"), Required(ErrorMessage = "*")]
    public string UserName { get; set; }
    [DataType(DataType.Password), Required(ErrorMessage = "*")]
    public string Password { get; set; }
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
