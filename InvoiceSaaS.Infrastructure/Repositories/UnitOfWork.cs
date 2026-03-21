using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class UnitOfWork(InvoiceSaaSDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Database.BeginTransactionAsync(cancellationToken);
    }
}
