using Microsoft.EntityFrameworkCore;
using Resort_Hub.Abstraction.Enums;
using Resort_Hub.Interfaces;
using Resort_Hub.ViewModels.Admin;

namespace Resort_Hub.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);
            var lastMonthEnd = currentMonthStart.AddDays(-1);

            // Get current month stats 
            var currentBookings = await _unitOfWork.Bookings
                .CountAsync(b => b.BookingDate >= currentMonthStart);

            var lastMonthBookings = await _unitOfWork.Bookings
                .CountAsync(b => b.BookingDate >= lastMonthStart && b.BookingDate <= lastMonthEnd);

            var currentUsers = await _unitOfWork.Users
                .CountAsync(u => u.CreatedAt >= currentMonthStart);

            var lastMonthUsers = await _unitOfWork.Users
                .CountAsync(u => u.CreatedAt >= lastMonthStart && u.CreatedAt <= lastMonthEnd);

            var currentRevenue = await _unitOfWork.Bookings
                .GetTotalRevenueAsync(currentMonthStart, null, VillaStatus.Approved);

            var lastMonthRevenue = await _unitOfWork.Bookings
                .GetTotalRevenueAsync(lastMonthStart, lastMonthEnd, VillaStatus.Approved);

            var averageBookingValue = currentBookings > 0 ? currentRevenue / currentBookings : 0;

            var totalVillas = await _unitOfWork.Villas.GetTotalVillasCountAsync();
            var totalBookings = await _unitOfWork.Bookings.CountAsync();
            var activeBookings = await _unitOfWork.Bookings.GetActiveBookingsCountAsync();
            var totalUsers = await _unitOfWork.Users.CountAsync();
            var totalRevenue = await _unitOfWork.Bookings.GetTotalRevenueAsync(null, null, VillaStatus.Approved);

            var occupancyRate = totalVillas > 0
                ? (double)activeBookings / totalVillas * 100
                : 0;

            var availableVillas = totalVillas - activeBookings;

            var completedBookings = await _unitOfWork.Bookings.GetBookingsByStatusCountAsync(VillaStatus.Completed);
            var cancelledBookings = await _unitOfWork.Bookings.GetBookingsByStatusCountAsync(VillaStatus.Cancelled);

            var bookingCompletionRate = totalBookings > 0
                ? (double)completedBookings / totalBookings * 100
                : 0;

            var chartData = await GetChartDataAsync(30);

            var recentBookingsList = await _unitOfWork.Bookings
                .GetBookingsWithDetailsAsync(0, 5);

            var recentBookings = recentBookingsList.Select(b => new BookingViewModel
            {
                Id = b.Id,
                UserFullName = $"{b.User?.FirstName} {b.User?.LastName}",
                UserEmail = b.User?.Email,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalCost = b.TotalCost,
                Status = b.Status
            }).ToList();

            var recentUsersList = await _unitOfWork.Users.GetRecentUsersAsync(5);
            var recentUsers = recentUsersList.Select(u => new RecentUserViewModel
            {
                UserId = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                JoinDate = u.CreatedAt,
                BookingsCount = _unitOfWork.Bookings.CountAsync(b => b.UserId == u.Id).Result
            }).ToList();

            return new DashboardViewModel
            {
                TotalBookings = totalBookings,
                BookingsChange = currentBookings - lastMonthBookings,
                TotalUsers = totalUsers,
                UsersChange = currentUsers - lastMonthUsers,
                TotalRevenue = totalRevenue,
                RevenueChange = currentRevenue - lastMonthRevenue,
                AverageBookingValue = averageBookingValue,
                ActiveBookings = activeBookings,
                OccupancyRate = occupancyRate,
                TotalVillas = totalVillas,
                AverageRating = 4.5,
                AvailableVillas = availableVillas,
                CompletedBookings = completedBookings,
                CancelledBookings = cancelledBookings,
                BookingCompletionRate = bookingCompletionRate,
                ChartData = chartData,
                RecentBookings = recentBookings,
                RecentUsers = recentUsers
            };
        }

        public async Task<ChartDataViewModel> GetChartDataAsync(int days = 30)
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-days + 1);

            var chartData = new ChartDataViewModel();

            var allDates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var bookingData = await _unitOfWork.Bookings
                .GetBookingsCountByDateRangeAsync(startDate, endDate);

            var memberData = await _unitOfWork.Users
                .GetUsersCountByDateRangeAsync(startDate, endDate);

            foreach (var date in allDates)
            {
                chartData.Dates.Add(date.ToString("MM/dd/yyyy"));
                chartData.BookingCounts.Add(bookingData.GetValueOrDefault(date, 0));
                chartData.MemberCounts.Add(memberData.GetValueOrDefault(date, 0));
            }

            return chartData;
        }

        public async Task<BookingListViewModel> GetBookingsAsync(int page = 1, int pageSize = 10, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
        {
            var skip = (page - 1) * pageSize;

            var totalCount = await _unitOfWork.Bookings.GetCountWithFilterAsync(search, status);

            // Get bookings with details
            var bookingsList = await _unitOfWork.Bookings
                .GetBookingsWithDetailsAsync(skip, pageSize, search, status, sortBy, descending);

            var bookings = bookingsList.Select(b => new BookingViewModel
            {
                Id = b.Id,
                BookingDate = b.BookingDate,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Nights = b.Nights,
                TotalCost = b.TotalCost,
                Status = b.Status,
                VillaId = b.VillaId,
                VillaName = b.Villa?.Name ?? "N/A",
                VillaImageUrl = "/images/resort-bg.jpg",
                UserId = b.UserId,
                UserFullName = $"{b.User?.FirstName} {b.User?.LastName}",
                UserEmail = b.User?.Email ?? string.Empty
            }).ToList();

            return new BookingListViewModel
            {
                Bookings = bookings,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                SearchTerm = search,
                StatusFilter = status,
                SortBy = sortBy,
                SortDescending = descending
            };
        }

        public async Task<BookingViewModel?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
            if (booking == null) return null;

            return new BookingViewModel
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                Nights = booking.Nights,
                TotalCost = booking.TotalCost,
                Status = booking.Status,
                VillaId = booking.VillaId,
                VillaName = booking.Villa?.Name ?? "N/A",
                VillaImageUrl = "/images/resort-bg.jpg",
                UserId = booking.UserId,
                UserFullName = $"{booking.User?.FirstName} {booking.User?.LastName}",
                UserEmail = booking.User?.Email ?? string.Empty
            };
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, VillaStatus newStatus, string? notes = null)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) return false;

            booking.Status = newStatus;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<int> GetBookingsCountAsync(VillaStatus? status = null)
        {
            if (status.HasValue)
                return await _unitOfWork.Bookings.CountAsync(b => b.Status == status.Value);
            return await _unitOfWork.Bookings.CountAsync();
        }

        public async Task<UserListViewModel> GetUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? role = null, string? sortBy = null, bool descending = false)
        {
            var skip = (page - 1) * pageSize;

            var totalCount = await _unitOfWork.Users.GetUsersCountWithFiltersAsync(search, role);

            var usersList = await _unitOfWork.Users.GetUsersWithFiltersAsync(skip, pageSize, search, role, sortBy, descending);

            var users = new List<UserViewModel>();

            foreach (var u in usersList)
            {
                var bookingsCount = await _unitOfWork.Users.GetUserBookingsCountAsync(u.Id);
                var roles = await _unitOfWork.Users.GetUserRolesAsync(u); 

                users.Add(new UserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber,
                    CreatedAt = u.CreatedAt,
                    EmailConfirmed = u.EmailConfirmed,
                    IsActive = true, 
                    Roles = roles,
                    BookingsCount = bookingsCount
                });
            }

            return new UserListViewModel
            {
                Users = users,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                SearchTerm = search,
                RoleFilter = role,
                SortBy = sortBy,
                SortDescending = descending
            };
        }
        public async Task<UserViewModel?> GetUserByIdAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;

            var bookingsCount = await _unitOfWork.Users.GetUserBookingsCountAsync(user.Id);
            var roles = await _unitOfWork.Users.GetUserRolesAsync(user); 

            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = true, 
                Roles = roles,
                BookingsCount = bookingsCount
            };
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            return await _unitOfWork.Users.UpdateUserRoleAsync(userId, role);
        }
        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            return await _unitOfWork.Users.ToggleUserStatusAsync(userId);
        }

      

    }
}