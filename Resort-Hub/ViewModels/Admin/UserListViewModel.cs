namespace Resort_Hub.ViewModels.Admin
{
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new();
        public int BookingsCount { get; set; }
        public string AvatarInitial => FullName.Length > 0 ? FullName.Substring(0, 1) : "?";

        public string StatusBadgeClass => IsActive ? "status-active" : "status-inactive";
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
    }

    public class UpdateUserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

 
}