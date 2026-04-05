using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Interfaces;

public interface ITaxService
{
    Task<IEnumerable<TaxDto>> GetTaxesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<TaxDto> CreateTaxAsync(Guid tenantId, CreateTaxDto dto, CancellationToken cancellationToken = default);
    Task UpdateTaxAsync(Guid tenantId, Guid id, CreateTaxDto dto, CancellationToken cancellationToken = default);
    Task DeleteTaxAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
}
