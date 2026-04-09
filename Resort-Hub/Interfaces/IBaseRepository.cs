namespace Resort_Hub.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    List<TEntity> GetAll();
    TEntity? GetById(int id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    bool Any(Func<TEntity, bool> predicate);
}
