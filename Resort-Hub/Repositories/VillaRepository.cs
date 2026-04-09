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
}
