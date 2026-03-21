using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;

namespace InvoiceSaaS.Application.Services;

public sealed class InvoiceService(IInvoiceRepository invoiceRepository) : IInvoiceService
{
    public async Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetAllByTenantAsync(tenantId, cancellationToken);

        return invoices
            .Select(x => new InvoiceDto
            {
                Id = x.Id,
                Number = x.Number,
                CustomerName = x.Customer?.DisplayName ?? string.Empty,
                Amount = x.Amount,
                DueDateUtc = x.DueDateUtc,
                Status = x.Status.ToString()
            })
            .ToArray();
    }
}
