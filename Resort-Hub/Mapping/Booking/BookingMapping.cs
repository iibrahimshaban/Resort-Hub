using Resort_Hub.ViewModels.Booking;

namespace Resort_Hub.Mapping.Booking
{
    public class BookingMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Entities.Booking, BookingPaymentPageViewModel>()
                  .Map(dest => dest.VillaId, src => src.Villa.Id)
                  .Map(dest => dest.Summary.Name, src => src.Villa.Name)
                  .Map(dest => dest.Summary.PricePerNight, src => src.Villa.PricePerNight)
                  .Map(dest => dest.Summary.Duration, src => src.Nights);

            config.NewConfig<Villa, BookingPreviewViewModel>()
                  .Map(dest => dest.Image, src => src.VillaImages.Where(img => img.IsMain)
                                                                 .Select(img => img.ImageUrl)
                                                                 .FirstOrDefault() ?? string.Empty);
        }
    }
}
