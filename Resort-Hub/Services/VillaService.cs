using Resort_Hub.Abstraction;
using Resort_Hub.Errors;
using Resort_Hub.Interfaces;

namespace Resort_Hub.Services;

public class VillaService(IUnitOfWork unitOfWork) : IVillaService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Villa>> ValidateVilla(int id)
    {
        var villa = await _unitOfWork.Villas.GetVillaAsNoTracking(id);

        if (villa == null)
            return Result.Failure<Villa>(VillaErrors.NotFound);

        return Result.Success(villa);
    }
}
