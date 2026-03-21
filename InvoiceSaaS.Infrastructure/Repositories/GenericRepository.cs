using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class GenericRepository<T>(InvoiceSaaSDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dbContext.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
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
