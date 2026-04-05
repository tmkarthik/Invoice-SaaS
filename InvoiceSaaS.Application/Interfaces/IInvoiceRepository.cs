using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<IReadOnlyCollection<Invoice>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
