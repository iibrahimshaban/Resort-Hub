using Resort_Hub.ViewModels.Admin;
using System.Threading.Tasks;

namespace ResortHub.Services
{
    public interface IAdminService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<ChartDataViewModel> GetChartDataAsync(int days = 30);
        Task<BookingListViewModel> GetBookingsAsync(int page = 1, int pageSize = 10, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false);
        Task<BookingViewModel?> GetBookingByIdAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int bookingId, VillaStatus newStatus, string? notes = null);
        Task<int> GetBookingsCountAsync(VillaStatus? status = null);
    }
}