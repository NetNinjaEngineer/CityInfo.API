using System.Linq.Expressions;

namespace CityInfo.API.Repository;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> FindAll(bool trackChanges, params Expression<Func<T, object>>[] includes);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges, params Expression<Func<T, object>>[] includes);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);
}
