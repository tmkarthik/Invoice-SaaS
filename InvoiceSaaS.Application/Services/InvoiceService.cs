using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IGenericRepository<Customer> customerRepository,
    IGenericRepository<Company> companyRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : IInvoiceService
{
    public async Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        var invoices = await invoiceRepository.GetAllByTenantAsync(tenantId, cancellationToken);

        return invoices.Select(MapToDto).ToArray();
    }

    public async Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        var invoices = await invoiceRepository.GetAllByTenantAsync(tenantId, cancellationToken);
        
        return invoices
            .Where(x => x.CompanyId == companyId)
            .Select(MapToDto)
            .ToArray();
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        
        if (dto.TenantId != tenantId && !tenantProvider.IsAdmin)
        {
            throw new UnauthorizedAccessException("Cannot create invoice for another tenant.");
        }

        // Step 10: Validation: Invoice TenantId matches: Customer TenantId, Company TenantId
        var customer = await customerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null) throw new KeyNotFoundException($"Customer {dto.CustomerId} not found.");
        if (customer.TenantId != dto.TenantId) throw new InvalidOperationException("Customer TenantId mismatch.");

        var company = await companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new KeyNotFoundException($"Company {dto.CompanyId} not found.");
        if (company.TenantId != dto.TenantId) throw new InvalidOperationException("Company TenantId mismatch.");

        var invoice = new Invoice(dto.TenantId, dto.CompanyId, dto.CustomerId, dto.Number, dto.IssueDate, dto.DueDate, dto.Currency);
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

        return MapToDto(invoice);
    }

    private static InvoiceDto MapToDto(Invoice x)
    {
        return new InvoiceDto
        {
            Id = x.Id,
            TenantId = x.TenantId,
            CompanyId = x.CompanyId,
            CustomerId = x.CustomerId,
            CustomerName = x.Customer?.Name ?? string.Empty,
            InvoiceNumber = x.Number,
            Amount = x.Amount,
            IssueDate = x.IssueDateUtc,
            DueDate = x.DueDateUtc,
            Currency = x.Currency,
            Status = x.Status.ToString()
        };
    }
}
