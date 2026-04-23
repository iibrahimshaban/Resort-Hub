using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using System.Linq.Expressions;

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
    }

    public bool Any(Func<T, bool> predicate)
    {
        return _entry.Any(predicate);
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
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
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _entry.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _entry.FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _entry.ToListAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entry.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entry.FirstOrDefaultAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _entry.CountAsync();
        return await _entry.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entry.AnyAsync(predicate);
    }

    public IQueryable<T> GetQueryable()
    {
        return _entry;
    }

    public T? GetById(string id)
    {
        throw new NotImplementedException();
    }

 
}
