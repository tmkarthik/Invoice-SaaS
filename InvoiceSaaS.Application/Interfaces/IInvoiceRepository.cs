using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<IReadOnlyCollection<Invoice>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
