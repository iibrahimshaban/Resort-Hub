using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Abstraction.Enums;
using Resort_Hub.ViewModels.Admin;
using Resort_Hub.Services;
using System.Threading.Tasks;

namespace Resort_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Bookings")]
    public class BookingsController : Controller
    {
        private readonly IAdminService _adminService;

        public BookingsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetBookingsAsync(page, 10, search, status, sortBy, descending);
            return View(model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _adminService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
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