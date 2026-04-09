namespace Resort_Hub.Interfaces;

public interface IVillaRepository : IBaseRepository<Villa>
{
    Task<Villa?> GetVillaAsNoTracking(int id);
}
