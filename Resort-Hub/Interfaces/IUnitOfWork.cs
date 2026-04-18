namespace Resort_Hub.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IVillaRepository Villas { get; }
    IBaseRepository<Amenity> Amenities { get; } 
    Task SaveAsync();
}
