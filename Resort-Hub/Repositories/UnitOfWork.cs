using Resort_Hub.Entities;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;

    private IVillaRepository? _villas = null;
    private IBookingRepository? _bookings = null;
    private IBaseRepository<Amenity> _amenities = null;
    private IUserRepository _users = null;
    
    
    public IVillaRepository Villas
    {
        get
        {
            if( _villas is null)
                _villas = new VillaRepository(_context);

            return _villas;
        }
    }
    public IBookingRepository Bookings
    {
        get
        {
            if(_bookings is null)
                _bookings = new BookingRepository(_context);

            return _bookings;
        }
    }


    public IBaseRepository<Amenity> Amenities 
    {
        get
        {
            if(_amenities is null)
                _amenities = new BaseRepository<Amenity>(_context);

            return _amenities;
        }
    } 

    public IUserRepository Users => 
    {
        get
        {
            if(_users is null)
                _users = new UserRepository(_context);

            return _users;
        }
    } 

    public void Dispose() => _context.Dispose();

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
