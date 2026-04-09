namespace Resort_Hub.Services;

public interface IVillaService
{
    Task<Villa?> ValidateVilla(int id);
}
