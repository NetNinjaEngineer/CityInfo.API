using CityInfo.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CityInfo.API.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    protected GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public void Create(T entity) => _dbSet.Add(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public IQueryable<T> FindAll(bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);

        return !trackChanges ? query.AsNoTracking() : query;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);

        return !trackChanges ?
        query.Where(expression)
        .AsNoTracking() : query.Where(expression);
    }

    public void Update(T entity) => _dbSet.Update(entity);
}
