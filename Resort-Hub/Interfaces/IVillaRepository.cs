using Resort_Hub.ViewModels.Home;

namespace Resort_Hub.Interfaces;

public interface IVillaRepository : IBaseRepository<Villa>
{
    Task<Villa?> GetVillaByIdWithPics(int id);
    Task<List<VillaVM>> GetVillasForHmePage();
    Task<List<VillaVM>> SearchVillas(DateOnly? checkInDate, int? personCount, string? location);
    Task<Villa?> IncludeAllData(int id);
    Task<Villa?> GetVillaAsNoTracking(int id);
    Task<int> GetTotalVillasCountAsync();
    Task<int> GetAvailableVillasCountAsync();
    Task<double> GetAverageRatingAsync();
}
