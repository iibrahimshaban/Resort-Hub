using Resort_Hub.ViewModels.Admin;
using System.Threading.Tasks;

namespace Resort_Hub.Services
{
    public interface IAdminService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<ChartDataViewModel> GetChartDataAsync(int days = 30);
        Task<BookingListViewModel> GetBookingsAsync(int page = 1, int pageSize = 10, string? search = null, VillaStatus? status = null, string? sortBy = null, bool descending = false);
        Task<BookingViewModel?> GetBookingByIdAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int bookingId, VillaStatus newStatus, string? notes = null);
        Task<int> GetBookingsCountAsync(VillaStatus? status = null);
        // User Management Methods
        Task<UserListViewModel> GetUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? role = null, string? sortBy = null, bool descending = false);
        Task<UserViewModel?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserRoleAsync(string userId, string role);
        Task<bool> ToggleUserStatusAsync(string userId);
    }
}