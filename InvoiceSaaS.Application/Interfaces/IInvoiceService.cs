using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken = default);
}
