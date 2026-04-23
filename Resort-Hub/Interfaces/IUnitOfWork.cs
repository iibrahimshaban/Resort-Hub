using Resort_Hub.Entities;

namespace Resort_Hub.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IVillaRepository Villas { get; }
    IBaseRepository<Amenity> Amenities { get; }
    IBookingRepository Bookings { get; }
    IUserRepository Users { get; }
    
    Task SaveAsync();
}
