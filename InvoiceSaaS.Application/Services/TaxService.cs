using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class TaxService(
    IGenericRepository<TaxDefinition> taxRepository,
    IUnitOfWork unitOfWork) : ITaxService
{
    public async Task<IEnumerable<TaxDto>> GetTaxesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var taxes = await taxRepository.GetAllAsync();
        return taxes
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Priority)
            .Select(x => new TaxDto
            {
                Id = x.Id,
                Name = x.Name,
                Rate = x.Rate,
                IsCompound = x.IsCompound,
                Priority = x.Priority
            });
    }

    public async Task<TaxDto> CreateTaxAsync(Guid tenantId, CreateTaxDto dto, CancellationToken cancellationToken = default)
    {
        var tax = new TaxDefinition(tenantId, dto.Name, dto.Rate, dto.IsCompound, dto.Priority);
        await taxRepository.AddAsync(tax);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaxDto
        {
            Id = tax.Id,
            Name = tax.Name,
            Rate = tax.Rate,
            IsCompound = tax.IsCompound,
            Priority = tax.Priority
        };
    }

    public async Task UpdateTaxAsync(Guid tenantId, Guid id, CreateTaxDto dto, CancellationToken cancellationToken = default)
    {
        var tax = await taxRepository.GetByIdAsync(id);
        if (tax == null || tax.TenantId != tenantId)
        {
            throw new KeyNotFoundException("Tax definition not found.");
        }

        tax.Update(dto.Name, dto.Rate, dto.IsCompound, dto.Priority);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTaxAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var tax = await taxRepository.GetByIdAsync(id);
        if (tax == null || tax.TenantId != tenantId)
        {
            throw new KeyNotFoundException("Tax definition not found.");
        }

        taxRepository.Delete(tax);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
