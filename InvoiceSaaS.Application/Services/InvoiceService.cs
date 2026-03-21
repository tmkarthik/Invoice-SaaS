using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork) : IInvoiceService
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

    public async Task<InvoiceDto> CreateInvoiceAsync(Guid tenantId, CreateInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        var invoice = new Invoice(tenantId, dto.CompanyId, dto.CustomerId, dto.Number, dto.IssueDateUtc, dto.DueDateUtc, dto.Currency);
        if (dto.Discount > 0)
        {
            invoice.SetDiscount(dto.Discount);
        }

        foreach (var itemDto in dto.Items)
        {
            var item = new InvoiceItem(invoice.Id, itemDto.ProductId, itemDto.Description, itemDto.Quantity, itemDto.UnitPrice, itemDto.TaxRate);
            invoice.AddItem(item);
        }

        await invoiceRepository.AddAsync(invoice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InvoiceDto
        {
            Id = invoice.Id,
            Number = invoice.Number,
            CustomerName = string.Empty,
            Amount = invoice.Amount,
            DueDateUtc = invoice.DueDateUtc,
            Status = invoice.Status.ToString()
        };
    }
}
