using Resort_Hub.Entities;
using Resort_Hub.Abstraction.Enums;

namespace Resort_Hub.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync(int skip, int take, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false);
        Task<int> GetCountWithFilterAsync(string? search = null, VillaStatus? status = null);
        Task<Booking?> GetBookingWithDetailsAsync(int id);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null, VillaStatus? status = null);
        Task<Dictionary<DateTime, int>> GetBookingsCountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetActiveBookingsCountAsync();
        Task<int> GetBookingsByStatusCountAsync(VillaStatus status);
    }
}