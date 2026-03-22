using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Persistence;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class UnitOfWork(InvoiceSaaSDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            return;

        await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.CurrentTransaction == null)
            return;

        await dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.CurrentTransaction == null)
            return;

        await dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }
}
