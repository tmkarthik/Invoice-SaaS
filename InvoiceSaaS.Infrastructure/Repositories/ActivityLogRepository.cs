using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class ActivityLogRepository(InvoiceSaaSDbContext context) 
    : GenericRepository<ActivityLog>(context), IActivityLogRepository
{
    public async Task<IReadOnlyCollection<ActivityLog>> GetRecentByTenantAsync(Guid tenantId, int count = 10)
    {
        return await context.ActivityLogs
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(count)
            .ToArrayAsync();
    }
}
