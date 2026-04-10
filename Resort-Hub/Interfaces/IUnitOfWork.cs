namespace Resort_Hub.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IVillaRepository Villas { get; }
}
