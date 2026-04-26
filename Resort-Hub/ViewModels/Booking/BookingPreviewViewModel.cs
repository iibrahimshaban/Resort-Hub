namespace Resort_Hub.ViewModels.Booking
{
    public class BookingPreviewViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double PricePerNight { get; set; }
        public required string Image { get; set; }
    }
}
