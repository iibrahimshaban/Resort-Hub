using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class VillaRepository(ApplicationDbContext dbContext) : BaseRepository<Villa>(dbContext), IVillaRepository
{
    public new List<Villa> GetAll()
    {
        return [.. _context.Villas
            .Include(v => v.VillaAmenity)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.VillaImages)];
    }

    public async Task<Villa?> GetVillaAsNoTracking(int id)
    {
        return await _context.Villas
            .Include(v => v.VillaAmenity)
            .Include(v => v.VillaImages)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }
}
