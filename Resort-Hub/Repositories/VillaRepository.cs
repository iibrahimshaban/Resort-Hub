using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class VillaRepository(ApplicationDbContext dbContext) : BaseRepository<Villa>(dbContext), IVillaRepository
{
    public async Task<Villa?> GetVillaAsNoTracking(int id)
    {
        var villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        return villa;
    }

    public async Task<Villa?> GetVillaByIdWithPics(int id)
    {
        var villa = await _context.Villas.Include(v => v.VillaImages.OrderBy(i => i.DispalayOrder))
                                         .FirstOrDefaultAsync(v => v.Id == id);
        return villa;
    }

    public async Task<Villa?> IncludeAllData(int id)
    {
        var villa = await _context.Villas.Include(v=> v.VillaAmenity)
                                         .ThenInclude(va=>va.Amenity)
                                         .Include(v=> v.VillaImages.OrderBy(i => i.DispalayOrder))
                                         .FirstOrDefaultAsync(v => v.Id == id);
        return villa;
    }
}
