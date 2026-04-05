using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Interfaces;

public interface ISettingsService
{
    Task<InvoiceSettingsDto> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(Guid tenantId, UpdateSettingsDto dto, CancellationToken cancellationToken = default);
}
