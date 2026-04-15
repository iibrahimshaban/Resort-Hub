using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Persistence;

namespace Resort_Hub.Controllers;

public class CustomValidationController(ApplicationDbContext dbContext) : Controller
{
    private readonly ApplicationDbContext _context = dbContext;

    [HttpGet]
    public IActionResult CheckEmail(string Email)
    {
        var users = _context.Users.Where(s => s.Email == Email);
        bool exists = users.Count() >= 1;

        return Json(!exists);
    }
}
