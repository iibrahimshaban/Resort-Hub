namespace Resort_Hub.ViewModels.Admin
{
    public class RecentUserViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public int BookingsCount { get; set; }
        public string UserFullName => FullName;
        public DateTime CreatedAt => JoinDate;
    }
}
