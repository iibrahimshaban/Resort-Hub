namespace Resort_Hub.ViewModels.Booking
{
    public class BookingPaymentPageViewModel
    {
        public int VillaId { get; set; }

        public BookingPaymentSummaryViewModel Summary { get; set; } = new();
        public DummyPaymentViewModel Payment { get; set; } = new();
    }
}
