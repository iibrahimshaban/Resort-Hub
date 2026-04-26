using Resort_Hub.ViewModels.Villa;


namespace Resort_Hub.Mapping.Villas
{
    public class VillaMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<VillaAmenity, VillaAmenityViewModel>()
                .Map(dest => dest.Name, src => src.Amenity.Name)
                .Map(dest => dest.Icon, src => src.Amenity.Icon);


            config.NewConfig<VillaImage, VillaImageViewModel>();

            config.NewConfig<Villa, VillaDetailsViewModel>()
                .Map(dest => dest.VillaAmenities, src => src.VillaAmenity);
        }
    }
}
