using Mapster;
using Resort_Hub.ViewModels.Admin;

namespace Resort_Hub.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<ApplicationUser, RecentUserViewModel>.NewConfig()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.FullName, src => src.FirstName + " " + src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.JoinDate, src => src.CreatedAt)
            .Map(dest => dest.BookingsCount, src => 0); 
    }
}
