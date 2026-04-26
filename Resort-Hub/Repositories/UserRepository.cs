using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count)
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            var roles = await (from ur in _context.UserRoles
                               join r in _context.Roles on ur.RoleId equals r.Id
                               where ur.UserId == user.Id
                               select r.Name).ToListAsync();
            return roles ?? new List<string>();
        }
        public async Task<Dictionary<DateTime, int>> GetUsersCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.Count);
        }

        public async Task<int> GetUsersCountByDateRangeCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Users
                .CountAsync(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate);
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersWithFiltersAsync(int skip, int take, string? search = null, string? role = null, string? sortBy = null, bool descending = false)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => _context.UserRoles
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                    .Any(x => x.UserId == u.Id && x.Name == role));
            }

            query = sortBy?.ToLower() switch
            {
                "fullname" => descending ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName) : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "createdat" => descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                "status" => descending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> GetUsersCountWithFiltersAsync(string? search = null, string? role = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => _context.UserRoles
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                    .Any(x => x.UserId == u.Id && x.Name == role));
            }

            return await query.CountAsync();
        }

        public async Task<ApplicationUser?> GetUserWithDetailsAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.IsActive = !user.IsActive;

            _context.Users.Update(user);


            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateUserRoleAsync(string userId, string roleName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) return false;

            var existingRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            _context.UserRoles.RemoveRange(existingRoles);

            _context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = role.Id
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUserBookingsCountAsync(string userId)
        {
            return await _context.Bookings.CountAsync(b => b.UserId == userId);
        }
    }
}