using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Interfaces;

public interface IActivityLogRepository : IGenericRepository<ActivityLog>
{
    Task<IReadOnlyCollection<ActivityLog>> GetRecentByTenantAsync(Guid tenantId, int count = 10);
}
