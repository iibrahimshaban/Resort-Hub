using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services;

namespace Resort_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Users")]
    public class UsersController : Controller
    {
        private readonly IAdminService _adminService;

        public UsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, string? role = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetUsersAsync(page, 10, search, role, sortBy, descending);
            return View("~/Views/Admin/Users/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return PartialView("~/Views/Admin/Users/Details.cshtml", user);
        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] string userId)
        {
            var result = await _adminService.ToggleUserStatusAsync(userId);
            return Ok(new { success = result });
        }
    }
}