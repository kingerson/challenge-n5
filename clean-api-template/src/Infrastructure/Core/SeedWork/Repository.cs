namespace MsClean.Infrastructure;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MsClean.Domain;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly ApplicationDbContext _context;
    private static readonly char[] separator = [','];

    public Repository(ApplicationDbContext context) => _context = context;

    public Task<List<T>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Set<T>().ToListAsync(cancellationToken);
    }

    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression, string includeProperties)
    {
        var query = _context.Set<T>().AsQueryable();

        query = includeProperties.Split(separator,
            StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty)
            => current.Include(includeProperty));

        return query.SingleOrDefaultAsync(expression);
    }
    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        var query = _context.Set<T>().AsQueryable();

        return query.SingleOrDefaultAsync(expression);
    }

    public IQueryable<T> FindQueryable(Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = _context.Set<T>().Where(expression);
        return orderBy != null ? orderBy(query) : query;
    }

    public Task<List<T>> FindListAsync(Expression<Func<T, bool>>? expression, Func<IQueryable<T>,
        IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = expression != null ? _context.Set<T>().Where(expression) : _context.Set<T>();
        return orderBy != null
            ? orderBy(query).ToListAsync(cancellationToken)
            : query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicado = null,
                                            List<Expression<Func<T, object>>>? includes = null,
                                            bool disableTracking = true)
    {
        IQueryable<T> query = _context.Set<T>();
        if (disableTracking)
            query = query.AsNoTracking();
        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        if (predicado != null)
            query = query.Where(predicado);
        return await query.ToListAsync();
    }

    public async Task<T?> GetById(int id) => await _context.Set<T>().FindAsync(id);

    public async Task<T> Add(T entity) => (await _context.Set<T>().AddAsync(entity)).Entity;

    public async Task AddRange(List<T> entities) => await _context.Set<T>().AddRangeAsync(entities);

    public void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateRange(IEnumerable<T> entities) => _context.Set<T>().UpdateRange(entities);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public void DeleteRange(List<T> entities) => _context.Set<T>().RemoveRange(entities);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await _context.Set<T>().AnyAsync(predicate, cancellationToken);

}
