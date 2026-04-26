using Resort_Hub.DTOs.Booking;
using System.Linq.Expressions;

namespace Resort_Hub.Services.Book
{
    public interface IBookingService
    {
        public Task<Result<Booking>> GetBookingAllDataByIdAsync(int villaId, string userId, VillaStatus status);
        public Task<Result<List<BookedDateDTO>>> GetVillaBookedDatesAsync(int villId);
        public Result<Villa> GetVillaById(int id);
        public Task<Result<Villa>> GetVillaByIdWithPicsAsync(int id);
        public Task<Result<bool>> IsBookOverlapping(Expression<Func<Booking, bool>> predicate);
        public Task<Result<Booking>> CheckConfirmed(int bookingId, string userID, VillaStatus villaStatus);
        public void AddBooking(Booking booking);
        public Task SaveChangesAsync();
        public void Update(Booking booking);
        public Task<Result<Booking>> GetDraftBookingAsync(int villaId, string userId);
    }
}
