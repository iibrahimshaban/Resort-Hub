using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class VillaRepository(ApplicationDbContext dbContext)
    : BaseRepository<Villa>(dbContext), IVillaRepository
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
                .ThenInclude(va => va.Amenity)
            .Include(v => v.VillaImages)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }
    public async Task<int> GetTotalVillasCountAsync()
    {
        return await _context.Villas.CountAsync();
    }

    public async Task<int> GetAvailableVillasCountAsync()
    {
        return await _context.Villas.CountAsync(v => v.IsAvilable);
    }

    public async Task<double> GetAverageRatingAsync()
    {
        return 4.5;
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
