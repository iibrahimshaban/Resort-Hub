using Resort_Hub.DTOs.Booking;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using System.Linq.Expressions;

namespace Resort_Hub.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : BaseRepository<Booking>(context), IBookingRepository
    {
        public async Task<List<BookedDateDTO>> GetBookedDatesByVillaIdAsync(int villaId)
        {
            return await context.Bookings.Where(b => b.VillaId == villaId &&
                                               (b.Status == VillaStatus.Approved ||b.Status == VillaStatus.CheckedIn))
                                         .Select(b => new BookedDateDTO
                                         {
                                             CheckInDate = b.CheckInDate,
                                             CheckOutDate = b.CheckOutDate
                                         }).ToListAsync();
        }


        public async Task<Booking?> GetBookedAllDataByIdAsync(int bookingId)
        {
            return await _context.Bookings.Include(x => x.Villa)
                                          .FirstOrDefaultAsync(x => x.Id == bookingId);

        }

        public async Task<Booking?> GetBookedAllDataByIdAsync(int bookingId, string userId)
        {
            return await _context.Bookings.Include(x => x.Villa)
                                          .FirstOrDefaultAsync(x => x.Id == bookingId &&
                                                                    x.UserId == userId);

        }

        public async Task<Booking?> GetBookedAllDataByIdAsync(int bookingId, string userId, VillaStatus status)
        {
            return await _context.Bookings.Include(x => x.Villa)
                                          .FirstOrDefaultAsync(x => x.Id == bookingId &&
                                                                    x.UserId == userId &&
                                                                    x.Status == status);

        }

        public async Task<bool> AnyAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _context.Bookings.AnyAsync(predicate);
        }

        public async Task<Booking?> GetDraftBookingAsync(int villaId, string userId)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.VillaId == villaId && b.UserId == userId && b.Status == VillaStatus.Draft);
        }
    }
}
