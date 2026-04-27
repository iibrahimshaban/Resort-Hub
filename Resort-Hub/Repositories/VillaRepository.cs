using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using Resort_Hub.ViewModels.Home;

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

    public async Task<List<VillaVM>> GetVillasForHmePage()
    {
        var topBookedIds = _context.Bookings
        .GroupBy(b => b.VillaId)
        .OrderByDescending(g => g.Count())
        .Take(4)
        .Select(g => g.Key)
        .ToHashSet();

        return await _context.Villas
            .Include(v => v.VillaImages)
            .Select(v => new VillaVM
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                PricePerNight = v.PricePerNight,
                Capacity = v.Capacity,
                IsAvailable = v.IsAvilable,
                IsPopularChoice = topBookedIds.Contains(v.Id),
                Location = v.Location,
                MainImageUrl = v.VillaImages
                                .FirstOrDefault(img => img.IsMain).ImageUrl
            })
            .ToListAsync();
    }

    public async Task<List<VillaVM>> SearchVillas(DateOnly? checkInDate,int? personCount,string? location)
    {
        var topBookedIds = (await _context.Bookings
        .GroupBy(b => b.VillaId)
        .OrderByDescending(g => g.Count())
        .Take(4)
        .Select(g => g.Key)
        .ToListAsync())
        .ToHashSet();

        var query = _context.Villas
            .Include(v => v.VillaImages)
            .Include(v => v.Bookings)
            .AsQueryable();

        // filter by capacity
        if (personCount.HasValue)
            query = query.Where(v => v.Capacity >= personCount.Value);

        // filter by location
        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(v => v.Location != null &&
                                     v.Location.Contains(location));

        // filter by availability (no overlapping confirmed bookings)
        if (checkInDate.HasValue)
            query = query.Where(v =>
                !v.Bookings.Any(b =>
                    b.Status != VillaStatus.Cancelled &&
                    b.CheckInDate <= checkInDate.Value &&
                    b.CheckOutDate > checkInDate.Value));

        return await query
            .Select(v => new VillaVM
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                PricePerNight = v.PricePerNight,
                Capacity = v.Capacity,
                IsAvailable = v.IsAvilable,
                Location = v.Location,
                IsPopularChoice = topBookedIds.Contains(v.Id),
                MainImageUrl = v.VillaImages
                                .FirstOrDefault(img => img.IsMain).ImageUrl
            })
            .ToListAsync();
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
