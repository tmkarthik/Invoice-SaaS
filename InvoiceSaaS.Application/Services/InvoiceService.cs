using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Application.Services;

public sealed class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IGenericRepository<Customer> customerRepository,
    IGenericRepository<Company> companyRepository,
    IActivityLogRepository activityLogRepository,
    IGenericRepository<InvoiceSettings> settingsRepository,
    IGenericRepository<TaxDefinition> taxRepository,
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

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        var invoice = await invoiceRepository.GetByIdAsync(id);
        
        if (invoice == null || (invoice.TenantId != tenantId && !tenantProvider.IsAdmin))
        {
            return null;
        }

        return MapToDto(invoice);
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

        var settings = (await settingsRepository.GetAllAsync())
            .FirstOrDefault(x => x.TenantId == tenantId);

        var number = dto.Number;
        if (string.IsNullOrWhiteSpace(number) && settings != null)
        {
            number = settings.GenerateNextInvoiceNumber();
        }

        var currency = dto.Currency ?? settings?.DefaultCurrency ?? "USD";
        var dueDate = dto.DueDate == default && settings != null 
            ? dto.IssueDate.AddDays(settings.DefaultDueDays) 
            : dto.DueDate;

        var invoice = new Invoice(dto.TenantId, dto.CompanyId, dto.CustomerId, number ?? "DRAFT", dto.IssueDate, dueDate, currency);
        
        if (dto.Discount > 0)
        {
            invoice.SetDiscount(dto.Discount);
        }

        foreach (var itemDto in dto.Items)
        {
            var item = new InvoiceItem(invoice.Id, itemDto.ProductId, itemDto.Description, itemDto.Quantity, itemDto.UnitPrice);
            if (tenantId != Guid.Empty) item.SetTenant(tenantId);

            if (itemDto.TaxIds != null && itemDto.TaxIds.Count > 0)
            {
                var appliedTaxes = (await taxRepository.GetAllAsync())
                    .Where(x => itemDto.TaxIds.Contains(x.Id))
                    .OrderBy(x => x.Priority);

                foreach (var taxDef in appliedTaxes)
                {
                    item.AddTax(taxDef);
                }
            }
            invoice.AddItem(item);
        }

        await invoiceRepository.AddAsync(invoice);
        
        var log = new ActivityLog(tenantId, "Invoice Created", $"Invoice #{invoice.Number} created for {customer.Name}", invoice.Id);
        await activityLogRepository.AddAsync(log);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(invoice);
    }

    public async Task BulkMarkAsPaidAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        foreach (var id in ids)
        {
            var invoice = await invoiceRepository.GetByIdAsync(id);
            if (invoice != null && invoice.TenantId == tenantId)
            {
                invoice.SetStatus(InvoiceSaaS.Domain.Enums.InvoiceStatus.Paid);
                await activityLogRepository.AddAsync(new ActivityLog(tenantId, "Invoice Paid", $"Invoice #{invoice.Number} marked as paid via bulk action", invoice.Id));
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task BulkDeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        foreach (var id in ids)
        {
            var invoice = await invoiceRepository.GetByIdAsync(id);
            if (invoice != null && invoice.TenantId == tenantId)
            {
                invoice.SoftDelete();
                await activityLogRepository.AddAsync(new ActivityLog(tenantId, "Invoice Deleted", $"Invoice #{invoice.Number} deleted via bulk action", invoice.Id));
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static InvoiceDto MapToDto(Invoice x)
    {
        var breakdown = new List<TaxBreakdownDto>();
        var items = new List<InvoiceItemDto>();

        if (x.InvoiceItems != null)
        {
            foreach (var item in x.InvoiceItems)
            {
                var itemTaxes = item.Taxes?.Select(t => new TaxBreakdownDto
                {
                    Name = t.TaxName,
                    Rate = t.AppliedRate,
                    Amount = t.Amount
                }).ToList() ?? new List<TaxBreakdownDto>();

                items.Add(new InvoiceItemDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Amount = item.Quantity * item.UnitPrice,
                    Taxes = itemTaxes
                });
            }

            var aggregatedTaxes = x.InvoiceItems
                .SelectMany(item => item.Taxes ?? Enumerable.Empty<InvoiceItemTax>())
                .GroupBy(t => new { t.TaxName, t.AppliedRate })
                .Select(g => new TaxBreakdownDto
                {
                    Name = g.Key.TaxName,
                    Rate = g.Key.AppliedRate,
                    Amount = g.Sum(t => t.Amount)
                }).ToList();

            breakdown.AddRange(aggregatedTaxes);
        }

        return new InvoiceDto
        {
            Id = x.Id,
            TenantId = x.TenantId,
            CompanyId = x.CompanyId,
            CustomerId = x.CustomerId,
            Customer = x.Customer != null ? new CustomerDto 
            { 
                Name = x.Customer.Name, 
                BillingAddress = x.Customer.CustomerAddresses
                    .Where(ca => ca.AddressType == AddressType.Billing && ca.IsDefault)
                    .Select(ca => ca.Address != null ? $"{ca.Address.AddressLine1}, {ca.Address.City}, {ca.Address.State} {ca.Address.PostalCode}" : null)
                    .FirstOrDefault()
            } : null,
            CustomerName = x.Customer?.Name ?? string.Empty,
            CustomerEmail = x.Customer?.Email ?? string.Empty,
            InvoiceNumber = x.Number,
            Subtotal = x.Subtotal,
            TaxTotal = x.TotalTax,
            Amount = x.Amount,
            TaxBreakdown = breakdown,
            Items = items,
            IssueDate = x.IssueDateUtc,
            DueDate = x.DueDateUtc,
            Currency = x.Currency,
            Status = x.Status.ToString()
        };
    }
}
