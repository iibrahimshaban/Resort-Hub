using Resort_Hub.Abstraction;

namespace Resort_Hub.Services;

public interface IVillaService
{
    Task<Result<Villa>> ValidateVilla(int id);
}
