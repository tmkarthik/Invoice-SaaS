using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
