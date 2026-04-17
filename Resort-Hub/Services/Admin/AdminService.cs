using Microsoft.EntityFrameworkCore;
using Resort_Hub.Persistence;
using Resort_Hub.ViewModels.Admin;


namespace ResortHub.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);
            var lastMonthEnd = currentMonthStart.AddDays(-1);

            // Get current month stats
            var currentBookings = await _context.Bookings
                .CountAsync(b => b.BookingDate >= currentMonthStart);

            var lastMonthBookings = await _context.Bookings
                .CountAsync(b => b.BookingDate >= lastMonthStart && b.BookingDate <= lastMonthEnd);

            //var currentUsers = await _context.Users
            //    .CountAsync(u => u. >= currentMonthStart);

            //var lastMonthUsers = await _context.Users
            //    .CountAsync(u => u.cr >= lastMonthStart && u.CreatedDate <= lastMonthEnd);

            var currentRevenue = await _context.Bookings
                .Where(b => b.BookingDate >= currentMonthStart && b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            var lastMonthRevenue = await _context.Bookings
                .Where(b => b.BookingDate >= lastMonthStart && b.BookingDate <= lastMonthEnd && b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            // Calculate averages
            var averageBookingValue = currentBookings > 0 ? currentRevenue / currentBookings : 0;

            // Get total counts
            var totalBookings = await _context.Bookings.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            // Get chart data
            var chartData = await GetChartDataAsync(30);

            // Get recent bookings (last 5)
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new RecentBookingViewModel
                {
                    BookingId = b.Id,
                    CustomerName = b.User.FirstName + " " + b.User.LastName,
                    CustomerEmail = b.User.Email,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalAmount = b.TotalCost,
                    Status = b.Status
                })
                .ToListAsync();

            // Get recent users (last 5)
            var recentUsers = await _context.Users
                .Take(5)
                .Select(u => new RecentUserViewModel
                {
                    UserId = u.Id,
                    FullName = u.FirstName+" "+u.LastName,
                    Email = u.Email,
                    JoinDate = new DateTime(2026,10,10),
                    BookingsCount = _context.Bookings.Count(b => b.UserId == u.Id)
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalBookings = totalBookings,
                BookingsChange = currentBookings - lastMonthBookings,
                TotalUsers = totalUsers,
                UsersChange = 20,
                TotalRevenue = totalRevenue,
                RevenueChange = currentRevenue - lastMonthRevenue,
                AverageBookingValue = averageBookingValue,
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

            // Get all dates in range
            var allDates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            // Get booking counts per day
            var bookingData = await _context.Bookings
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);

            //var memberData = await _context.Users
            //    .Where(u => u. >= startDate && u.CreatedDate <= endDate)
            //    .GroupBy(u => u.CreatedDate.Date)
            //    .Select(g => new { Date = g.Key, Count = g.Count() })
            //    .ToDictionaryAsync(g => g.Date, g => g.Count);

            foreach (var date in allDates)
            {
                chartData.Dates.Add(date.ToString("MM/dd/yyyy"));
                chartData.BookingCounts.Add(bookingData.GetValueOrDefault(date, 0));
            }

            return chartData;
        }
    }
}