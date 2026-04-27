namespace Resort_Hub.ViewModels.Booking;

public class UserBookingVM
{
    public int Id { get; set; }
    public int VillaId { get; set; }
    // Villa info
    public string VillaName { get; set; }
    public string VillaImage { get; set; }
    public string VillaLocation { get; set; }

    // Booking info
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Nights => (CheckOutDate.ToDateTime(TimeOnly.MinValue) - CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
    public decimal TotalCost { get; set; }
    public VillaStatus Status { get; set; }
    public DateTime BookingDate { get; set; }

    // Computed for view logic
    public bool CanCancel => Status == VillaStatus.Pending ||
                             Status == VillaStatus.Approved;
    public bool CanReview => Status == VillaStatus.Completed;
}
