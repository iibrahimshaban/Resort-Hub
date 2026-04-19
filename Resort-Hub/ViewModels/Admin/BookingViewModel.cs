namespace Resort_Hub.ViewModels.Admin
{
    public class BookingViewModel
    {
       
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int Nights { get; set; }
        public decimal TotalCost { get; set; }
        public VillaStatus Status { get; set; }
        public string StatusDisplay => GetStatusDisplay(Status);
        public string StatusBadgeClass => GetStatusBadgeClass(Status);

        public int VillaId { get; set; }
        public string VillaName { get; set; } = string.Empty;
        public string VillaImageUrl { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        private string GetStatusDisplay(VillaStatus status)
        {
            return status switch
            {
                VillaStatus.Pending => "Pending",
                VillaStatus.Approved => "Approved",
                VillaStatus.CheckedIn => "Checked-In",
                VillaStatus.Completed => "Completed",
                VillaStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }

        private string GetStatusBadgeClass(VillaStatus status)
        {
            return status switch
            {
                VillaStatus.Pending => "status-pending",
                VillaStatus.Approved => "status-approved",
                VillaStatus.CheckedIn => "status-checkedin",
                VillaStatus.Completed => "status-completed",
                VillaStatus.Cancelled => "status-cancelled",
                _ => "status-unknown"
            };
        }
    }

    public class UpdateBookingStatusViewModel
    {
        public int BookingId { get; set; }
        public VillaStatus NewStatus { get; set; }
    }
}

