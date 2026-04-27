namespace Resort_Hub.ViewModels.Booking
{
    public class BookingPaymentSummaryViewModel
    {
        public string Name { get; set; } = "";
        public double PricePerNight { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; } = string.Empty;

    }
}
