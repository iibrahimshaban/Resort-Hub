using Microsoft.AspNetCore.Mvc;

namespace Resort_Hub.ViewModels.Account;

public class UpdateProfileVM
{
    [Remote(action: "CheckUsername", controller: "CustomValidation", ErrorMessage = "Username is already taken.")]
    public string UserName { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
}
