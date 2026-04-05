using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class EfInvoiceRepository(InvoiceSaaSDbContext dbContext) 
    : GenericRepository<Invoice>(dbContext), IInvoiceRepository
{
    public async Task<IReadOnlyCollection<Invoice>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .AsNoTracking()
            .Include(x => x.Customer)
                .ThenInclude(c => c!.CustomerAddresses)
                    .ThenInclude(ca => ca.Address)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Taxes)
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedDate)
            .ToArrayAsync(cancellationToken);
    }

    public override async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .Include(x => x.Customer)
                .ThenInclude(c => c!.CustomerAddresses)
                    .ThenInclude(ca => ca.Address)
            .Include(x => x.Company)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Taxes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
