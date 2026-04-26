namespace Resort_Hub.Services;

public interface IVillaService
{
    Task<Result<Villa>> GetAllVillaData(int id);
    Task<Result<Villa>> ValidateVilla(int id);
}
