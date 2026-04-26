using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Resort_Hub.Abstraction.Enums;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Admin;

namespace Resort_Hub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Dashboard

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

        #endregion

        #region User Management

        [HttpGet]
        public async Task<IActionResult> Users(int page = 1, int pageSize = 10, string? search = null, string? role = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetUsersAsync(page, pageSize, search, role, sortBy, descending);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_UserDetailsPartial", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Role))
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == model.UserId)
            {
                return Json(new { success = false, message = "You cannot change your own role" });
            }

            var result = await _adminService.UpdateUserRoleAsync(model.UserId, model.Role);

            if (result)
            {
                return Json(new { success = true, message = "User role updated successfully" });
            }

            return Json(new { success = false, message = "Failed to update user role" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var result = await _adminService.ToggleUserStatusAsync(userId);

            if (result)
            {
                return Json(new { success = true, message = "User status updated successfully" });
            }

            return Json(new { success = false, message = "Failed to update user status" });
        }



        #endregion

        #region Booking Management

        [HttpGet]
        public async Task<IActionResult> Bookings(int page = 1, int pageSize = 10, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetBookingsAsync(page, pageSize, search, status, sortBy, descending);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var booking = await _adminService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return PartialView("_BookingDetailsPartial", booking);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, VillaStatus newStatus, string? notes = null)
        {
            var result = await _adminService.UpdateBookingStatusAsync(bookingId, newStatus, notes);

            if (result)
            {
                return Json(new { success = true, message = "Booking status updated successfully" });
            }

            return Json(new { success = false, message = "Failed to update booking status" });
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId, string? reason = null)
        {
            var result = await _adminService.UpdateBookingStatusAsync(bookingId, VillaStatus.Cancelled, reason);

            if (result)
            {
                return Json(new { success = true, message = "Booking cancelled successfully" });
            }

            return Json(new { success = false, message = "Failed to cancel booking" });
        }


        #endregion


    

    }
}