using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class EfInvoiceRepository(InvoiceSaaSDbContext dbContext) : IInvoiceRepository
{
    public async Task<IReadOnlyCollection<Invoice>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.InvoiceItems)
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedDate)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
