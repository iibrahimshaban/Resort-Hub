namespace Resort_Hub.Interfaces;

public interface IVillaRepository : IBaseRepository<Villa>
{
    Task<Villa?> GetVillaAsNoTracking(int id);
    Task<int> GetTotalVillasCountAsync();
    Task<int> GetAvailableVillasCountAsync();
    Task<double> GetAverageRatingAsync();
}
