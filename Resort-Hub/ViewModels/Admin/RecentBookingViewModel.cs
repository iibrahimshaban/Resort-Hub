namespace Resort_Hub.ViewModels.Admin
{
    public class RecentBookingViewModel
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public VillaStatus Status { get; set; }
    }
}
