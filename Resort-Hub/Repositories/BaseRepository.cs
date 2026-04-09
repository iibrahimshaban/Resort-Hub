using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;

namespace Resort_Hub.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entry;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
        _entry = _context.Set<T>();
    }
    public void Add(T entity)
    {
        _entry.Add(entity);
        _context.SaveChanges();
    }

    public bool Any(Func<T, bool> predicate)
    {
        return _entry.Any(predicate);
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public List<T> GetAll()
    {
        return [.. _entry];
    }

    public T? GetById(int id)
    {
        return _entry.Find(id);
    }

    public void Update(T entity)
    {
        _context.Update(entity);
        _context.SaveChanges();
    }
}
