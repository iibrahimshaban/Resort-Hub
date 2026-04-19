using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Admin;

namespace Resort_Hub.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var model = await _adminService.GetDashboardDataAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(int days = 30)
        {
            var chartData = await _adminService.GetChartDataAsync(days);
            return Json(chartData);
        }
    }
}