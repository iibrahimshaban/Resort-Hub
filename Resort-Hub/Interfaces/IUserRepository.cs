namespace Resort_Hub.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count);
        Task<Dictionary<DateTime, int>> GetUsersCountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetUsersCountByDateRangeCountAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ApplicationUser>> GetUsersWithFiltersAsync(int skip, int take, string? search = null, string? role = null, string? sortBy = null, bool descending = false);
        Task<int> GetUsersCountWithFiltersAsync(string? search = null, string? role = null);
        Task<ApplicationUser?> GetUserWithDetailsAsync(string userId);
        Task<bool> ToggleUserStatusAsync(string userId);
        Task<int> GetUserBookingsCountAsync(string userId);

        Task<List<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> UpdateUserRoleAsync(string userId, string roleName);
    }
}