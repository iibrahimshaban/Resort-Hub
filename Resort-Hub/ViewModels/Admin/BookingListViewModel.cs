namespace Resort_Hub.ViewModels.Admin
{
    public class BookingListViewModel
    {
        public List<BookingViewModel> Bookings { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public string? SearchTerm { get; set; }
        public VillaStatus? StatusFilter { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
