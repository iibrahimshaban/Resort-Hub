using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.EntityFrameworkCore;
using Resort_Hub.Abstraction;
using Resort_Hub.Entities;
using Resort_Hub.Errors;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Services;

public class VillaService(IUnitOfWork unitOfWork, ApplicationDbContext context) : IVillaService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationDbContext _context = context;

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

    public async Task<Result<Villa>> GetVillaForEdit(int id)
    {
        var villa = await _context.Villas
            .Include(v => v.VillaAmenity)
            .Include(v => v.VillaImages)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (villa == null)
            return Result.Failure<Villa>(VillaErrors.NotFound);

        return Result.Success(villa);
    }
}
