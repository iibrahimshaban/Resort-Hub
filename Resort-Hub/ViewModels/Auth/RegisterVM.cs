using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Auth;

public class RegisterVM
{
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Remote(action: "CheckEmail",controller: "CustomValidation",ErrorMessage ="Email already exists")]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password), Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Password doen't match confirm password ")]
    public string ConfirmPassword { get; set; }

}