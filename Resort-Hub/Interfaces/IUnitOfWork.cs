using Resort_Hub.Entities;

namespace Resort_Hub.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IVillaRepository Villas { get; }
    IBookingRepository Bookings { get; }
    IBaseRepository<Amenity> Amenities { get; }
    IUserRepository Users { get; }
    Task SaveAsync();
}
