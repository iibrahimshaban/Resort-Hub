using Microsoft.AspNetCore.Mvc;

namespace Resort_Hub.ViewModels.Account;

public class UpdateProfileVM
{
    [Remote(action: "CheckUsername", controller: "CustomValidation", ErrorMessage = "Username is already taken.")]
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
