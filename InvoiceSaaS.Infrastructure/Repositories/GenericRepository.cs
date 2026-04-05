using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Repositories;

public class GenericRepository<T>(InvoiceSaaSDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
{
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public IQueryable<T> GetQueryable()
    {
        return dbContext.Set<T>().AsQueryable();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }
}
