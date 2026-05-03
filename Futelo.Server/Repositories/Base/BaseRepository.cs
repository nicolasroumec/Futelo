using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Futelo.Server.Data;
using System.Linq.Expressions;

namespace Futelo.Server.Repositories.Base;

public abstract class BaseRepository<T> where T : class
{
    protected FuteloContext Context { get; }

    protected BaseRepository(FuteloContext context)
    {
        Context = context;
    }

    protected IQueryable<T> FindAll()
    {
        return Context.Set<T>().AsNoTrackingWithIdentityResolution();
    }

    protected IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    {
        IQueryable<T> queryable = Context.Set<T>();

        if (includes != null)
            queryable = includes(queryable);

        return queryable.AsNoTrackingWithIdentityResolution();
    }

    protected IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return Context.Set<T>().Where(expression).AsNoTrackingWithIdentityResolution();
    }

    protected void Create(T entity) => Context.Set<T>().Add(entity);

    protected void Update(T entity) => Context.Entry(entity).State = EntityState.Modified;

    protected void Delete(T entity) => Context.Set<T>().Remove(entity);

    protected async Task SaveChangesAsync() => await Context.SaveChangesAsync();
}
