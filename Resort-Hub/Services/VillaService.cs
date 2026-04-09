using Resort_Hub.Interfaces;

namespace Resort_Hub.Services;

public class VillaService(IVillaRepository villaRepository) : IVillaService
{
    private readonly IVillaRepository _villaRepository = villaRepository;

    public async Task<Villa?> ValidateVilla(int id)
    {
        return await _villaRepository.GetVillaAsNoTracking(id);
    }
}
