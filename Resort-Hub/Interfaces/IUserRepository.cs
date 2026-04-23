using Resort_Hub.Entities;

namespace Resort_Hub.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count);
        Task<Dictionary<DateTime, int>> GetUsersCountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetUsersCountByDateRangeCountAsync(DateTime startDate, DateTime endDate);
    }
}