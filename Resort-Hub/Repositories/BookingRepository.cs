using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using Resort_Hub.Abstraction.Enums;

namespace Resort_Hub.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync(int skip, int take, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false)
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
                _ => query.OrderByDescending(b => b.BookingDate)
            };

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> GetCountWithFilterAsync(string? search = null, VillaStatus? status = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.User.FirstName.Contains(search) ||
                    b.User.LastName.Contains(search) ||
                    b.User.Email.Contains(search) ||
                    b.Villa.Name.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Villa)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null, VillaStatus? status = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(b => b.BookingDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(b => b.BookingDate <= endDate.Value);

            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);

            return await query.SumAsync(b => (decimal?)b.TotalCost) ?? 0;
        }

        public async Task<Dictionary<DateTime, int>> GetBookingsCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);
        }

        public async Task<int> GetActiveBookingsCountAsync()
        {
            return await _context.Bookings
                .CountAsync(b => b.Status == VillaStatus.Approved || b.Status == VillaStatus.CheckedIn);
        }

        public async Task<int> GetBookingsByStatusCountAsync(VillaStatus status)
        {
            return await _context.Bookings.CountAsync(b => b.Status == status);
        }
    }
}