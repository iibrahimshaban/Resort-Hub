namespace Resort_Hub.Interfaces;

public interface IVillaRepository : IBaseRepository<Villa>
{
    Task<Villa?> GetVillaByIdWithPics(int id);
    Task<Villa?> IncludeAllData(int id);
    Task<Villa?> GetVillaAsNoTracking(int id);
}
