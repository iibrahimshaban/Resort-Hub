using System.Linq.Expressions;

namespace Resort_Hub.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    List<TEntity> GetAll();
    TEntity? GetById(int id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    bool Any(Func<TEntity, bool> predicate);
    TEntity? GetById(string id);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByIdAsync(string id);
    Task<List<TEntity>> GetAllAsync();
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetQueryable();
}

