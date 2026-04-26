using Elfie.Serialization;
using Resort_Hub.DTOs.Booking;
using System.Linq.Expressions;

namespace Resort_Hub.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<bool> AnyAsync(Expression<Func<Booking, bool>> predicate);
        Task<List<BookedDateDTO>> GetBookedDatesByVillaIdAsync(int villaId);
        Task<Booking?> GetDraftBookingAsync(int villaId, string userId);
        Task<Booking?> GetBookedAllDataByIdAsync(int bookingId);
        Task<Booking?> GetBookedAllDataByIdAsync(int bookingId, string userId);
        Task<Booking?> GetBookedAllDataByIdAsync(int bookingId, string userId, VillaStatus status);
    }
}
