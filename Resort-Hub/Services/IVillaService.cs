using Resort_Hub.Abstraction;
using Resort_Hub.Entities;

namespace Resort_Hub.Services;

public interface IVillaService
{
    Task<Result<Villa>> ValidateVilla(int id);
    Task<Result<Villa>> GetVillaForEdit(int id);
}
