using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.ViewModels.Admin;
using Resort_Hub.Services;

namespace Resort_Hub.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Bookings")]
    public class BookingController : Controller
    {
        private readonly IAdminService _adminService;

        public BookingController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetBookingsAsync(page, 10, search, status, sortBy, descending);
            return View("~/Views/Admin/Booking/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _adminService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return PartialView("~/Views/Admin/Booking/Details.cshtml", booking);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateBookingStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.UpdateBookingStatusAsync(model.BookingId, model.NewStatus);

            if (!result)
            {
                return NotFound(new { success = false, message = "Booking not found" });
            }

            return Ok(new { success = true, message = "Booking status updated successfully" });
        }

        [HttpGet("GetStatusCounts")]
        public async Task<IActionResult> GetStatusCounts()
        {
            var counts = new
            {
                Pending = await _adminService.GetBookingsCountAsync(VillaStatus.Pending),
                Approved = await _adminService.GetBookingsCountAsync(VillaStatus.Approved),
                CheckedIn = await _adminService.GetBookingsCountAsync(VillaStatus.CheckedIn),
                Completed = await _adminService.GetBookingsCountAsync(VillaStatus.Completed),
                Cancelled = await _adminService.GetBookingsCountAsync(VillaStatus.Cancelled),
                Total = await _adminService.GetBookingsCountAsync()
            };

            return Ok(counts);
        }
    }
}