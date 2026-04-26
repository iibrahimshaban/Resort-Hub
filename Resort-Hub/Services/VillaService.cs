using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Interfaces;

namespace Resort_Hub.Services;

public class VillaService(IUnitOfWork unitOfWork) : IVillaService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Villa>> GetAllVillaData(int id)
    {
        var villa = await _unitOfWork.Villas.IncludeAllData(id);

        if (villa == null)
            return Result.Failure<Villa>(VillaErrors.NotFound);

        return Result.Success(villa);
    }

    public async Task<Result<Villa>> ValidateVilla([FromRoute] int id)
    {
        var villa = await _unitOfWork.Villas.GetVillaAsNoTracking(id);

        if (villa == null)
            return Result.Failure<Villa>(VillaErrors.NotFound);

        return Result.Success(villa);
    }
}
