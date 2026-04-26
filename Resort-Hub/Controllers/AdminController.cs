using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort_Hub.Abstraction.Enums;
using Resort_Hub.Entities;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Admin;

namespace Resort_Hub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _adminService = adminService;
            _userManager = userManager;
            _roleManager = roleManager;
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
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIds = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIds.Contains(u.Id));
            }

            query = sortBy?.ToLower() switch
            {
                "fullname" => descending
                    ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                    : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "email" => descending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "createdat" => descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var bookingsCount = await _adminService.GetUserBookingsCountAsync(user.Id);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    BookingsCount = bookingsCount
                });
            }

            var model = new UserListViewModel
            {
                Users = userViewModels,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                SearchTerm = search,
                RoleFilter = role,
                SortBy = sortBy,
                SortDescending = descending
            };

            return View("~/Views/Admin/Users/Index.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);
            var bookingsCount = await _adminService.GetUserBookingsCountAsync(user.Id);

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                user.Email,
                user.PhoneNumber,
                user.CreatedAt,
                user.IsActive,
                user.EmailConfirmed,
                Roles = roles,
                BookingsCount = bookingsCount,
                AvatarInitial = user.FirstName?.Length > 0 ? user.FirstName.Substring(0, 1) : "?"
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
        {
            if (string.IsNullOrEmpty(request?.UserId) || string.IsNullOrEmpty(request?.Role))
                return BadRequest(new { success = false, message = "User ID and Role are required" });

            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
                return BadRequest(new { success = false, message = $"Role '{request.Role}' does not exist" });

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    return BadRequest(new { success = false, message = $"Failed to remove current roles: {errors}" });
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return BadRequest(new { success = false, message = $"Failed to assign role '{request.Role}': {errors}" });
            }

            return Ok(new { success = true, message = $"User role updated to '{request.Role}' successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusRequest request)
        {
            if (string.IsNullOrEmpty(request?.UserId))
                return BadRequest(new { success = false, message = "User ID is required" });

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { success = true, isActive = user.IsActive, message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully" });

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = $"Failed to update user status: {errors}" });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { success = false, message = "User ID is required" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { success = true, roles = roles });
        }

        #endregion

        #region Booking Management

        [HttpGet]
        public async Task<IActionResult> Bookings(int page = 1, int pageSize = 10, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
        {
            var model = await _adminService.GetBookingsAsync(page, pageSize, search, status, sortBy, descending);
            return View("~/Views/Admin/Booking/Index.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var booking = await _adminService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return PartialView("~/Views/Admin/Booking/_BookingDetailsPartial.cshtml", booking);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusRequest request)
        {
            if (request == null || request.BookingId <= 0)
                return BadRequest(new { success = false, message = "Invalid booking ID" });

            var result = await _adminService.UpdateBookingStatusAsync(request.BookingId, request.NewStatus, request.Notes);

            if (result)
                return Ok(new { success = true, message = "Booking status updated successfully" });

            return NotFound(new { success = false, message = "Booking not found" });
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            if (request == null || request.BookingId <= 0)
                return BadRequest(new { success = false, message = "Invalid booking ID" });

            var result = await _adminService.UpdateBookingStatusAsync(request.BookingId, VillaStatus.Cancelled, request.Reason);

            if (result)
                return Ok(new { success = true, message = "Booking cancelled successfully" });

            return NotFound(new { success = false, message = "Booking not found" });
        }

        #endregion

    }
    }