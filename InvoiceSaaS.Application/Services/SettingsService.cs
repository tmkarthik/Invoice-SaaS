using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class SettingsService(
    IGenericRepository<InvoiceSettings> settingsRepository,
    IUnitOfWork unitOfWork) : ISettingsService
{
    public async Task<InvoiceSettingsDto> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var settings = (await settingsRepository.GetAllAsync())
            .FirstOrDefault(x => x.TenantId == tenantId);

        if (settings == null)
        {
            throw new KeyNotFoundException("Invoice settings not found for this tenant.");
        }

        return MapToDto(settings);
    }

    public async Task UpdateSettingsAsync(Guid tenantId, UpdateSettingsDto dto, CancellationToken cancellationToken = default)
    {
        var settings = (await settingsRepository.GetAllAsync())
            .FirstOrDefault(x => x.TenantId == tenantId);

        if (settings == null)
        {
            throw new KeyNotFoundException("Invoice settings not found for this tenant.");
        }

        settings.Update(
            dto.Prefix,
            dto.Suffix,
            dto.NextInvoiceNumber,
            dto.DefaultCurrency,
            dto.DefaultTaxRate,
            dto.DefaultDueDays,
            dto.LogoUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static InvoiceSettingsDto MapToDto(InvoiceSettings x)
    {
        return new InvoiceSettingsDto
        {
            Id = x.Id,
            CompanyId = x.CompanyId,
            Prefix = x.Prefix,
            Suffix = x.Suffix,
            NextInvoiceNumber = x.CurrentNumber + 1,
            DefaultCurrency = x.DefaultCurrency,
            DefaultTaxRate = x.DefaultTaxRate,
            DefaultDueDays = x.DefaultDueDays,
            LogoUrl = x.LogoUrl
        };
    }
}
