using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public IVillaRepository Villas => new VillaRepository(_context);

    public IBaseRepository<Amenity> Amenities => new BaseRepository<Amenity>(_context);

    public void Dispose()
    {
        _context.Dispose();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
