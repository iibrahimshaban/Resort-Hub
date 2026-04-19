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

            var currentUsers = await _context.Users
                .CountAsync(u => u.CreatedAt >= currentMonthStart);

            var lastMonthUsers = await _context.Users
                .CountAsync(u => u.CreatedAt>= lastMonthStart && u.CreatedAt <= lastMonthEnd);

            var currentRevenue = await _context.Bookings
                .Where(b => b.BookingDate >= currentMonthStart && b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            var lastMonthRevenue = await _context.Bookings
                .Where(b => b.BookingDate >= lastMonthStart && b.BookingDate <= lastMonthEnd && b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            // Calculate averages
            var averageBookingValue = currentBookings > 0 ? currentRevenue / currentBookings : 0;

            // Get total counts
            var totalVillas = await _context.Villas.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var activeBookings = await _context.Bookings
            .CountAsync(b => b.Status == VillaStatus.Approved || b.Status == VillaStatus.CheckedIn);
            var totalUsers = await _context.Users.CountAsync();
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status == VillaStatus.Approved)
                .SumAsync(b => (decimal?)b.TotalCost) ?? 0;

            var occupancyRate = totalVillas > 0
               ? (double)activeBookings / totalVillas * 100
               : 0;

            // Calculate Average Rating from Villas
            
            var availableVillas = totalVillas - activeBookings;

            // Calculate Completed and Cancelled Bookings
            var completedBookings = await _context.Bookings
                .CountAsync(b => b.Status == VillaStatus.Completed);
            var cancelledBookings = await _context.Bookings
                .CountAsync(b => b.Status == VillaStatus.Cancelled);

            // Calculate Booking Completion Rates
            var bookingCompletionRate = totalBookings > 0
                ? (double)completedBookings / totalBookings * 100
                : 0;
            // Get chart data
            var chartData = await GetChartDataAsync(30);

            // Get recent bookings (last 5)
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    UserFullName = b.User.FirstName + " " + b.User.LastName,
                    UserEmail = b.User.Email,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalCost = b.TotalCost,
                    Status = b.Status
                })
                .ToListAsync();

            // Get recent users (last 5)
            var recentUsers = await _context.Users
                .Take(5)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new RecentUserViewModel
                {
                    UserId = u.Id,
                    FullName = u.FirstName+" "+u.LastName,
                    Email = u.Email,
                    JoinDate = u.CreatedAt,
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

            var allDates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var bookingData = await _context.Bookings
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);

            var memberData = await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);

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
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Villa)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.User.FirstName.Contains(search) ||
                    b.User.LastName.Contains(search) ||
                    b.User.Email.Contains(search) ||
                    b.Villa.Name.Contains(search));
            }

            // Apply status filter
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "bookingdate" => descending ? query.OrderByDescending(b => b.BookingDate) : query.OrderBy(b => b.BookingDate),
                "checkindate" => descending ? query.OrderByDescending(b => b.CheckInDate) : query.OrderBy(b => b.CheckInDate),
                "checkoutdate" => descending ? query.OrderByDescending(b => b.CheckOutDate) : query.OrderBy(b => b.CheckOutDate),
                "totalcost" => descending ? query.OrderByDescending(b => b.TotalCost) : query.OrderBy(b => b.TotalCost),
                "status" => descending ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status),
                "villa" => descending ? query.OrderByDescending(b => b.Villa.Name) : query.OrderBy(b => b.Villa.Name),
                "customer" => descending ? query.OrderByDescending(b => b.User.FirstName) : query.OrderBy(b => b.User.FirstName),
                _ => query.OrderByDescending(b => b.BookingDate) // Default sort by newest
            };

            var totalCount = await query.CountAsync();

            var bookings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    Nights = b.Nights,
                    TotalCost = b.TotalCost,
                    Status = b.Status,
                    VillaId = b.VillaId,
                    VillaName = b.Villa.Name,
                    VillaImageUrl =  "/images/resort-bg.jpg",
                    UserId = b.UserId,
                    UserFullName = $"{b.User.FirstName} {b.User.LastName}",
                    UserEmail = b.User.Email ?? string.Empty
                })
                .ToListAsync();

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
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Villa)
                .FirstOrDefaultAsync(b => b.Id == id);

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
                VillaName = booking.Villa.Name,
                VillaImageUrl = "/images/resort-bg.jpg",
                UserId = booking.UserId,
                UserFullName = $"{booking.User.FirstName} {booking.User.LastName}",
                UserEmail = booking.User.Email ?? string.Empty
            };
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, VillaStatus newStatus, string? notes = null)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = newStatus;

            // Optionally log the status change
            if (!string.IsNullOrWhiteSpace(notes))
            {
                // You can add audit logging here
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetBookingsCountAsync(VillaStatus? status = null)
        {
            var query = _context.Bookings.AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }
            return await query.CountAsync();
        }

    }
}