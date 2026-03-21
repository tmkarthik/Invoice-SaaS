using InvoiceSaaS.Application.DTOs.Template;

namespace InvoiceSaaS.Application.Interfaces;

public interface ITemplateService
{
    Task<TemplateDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TemplateDto>> GetAllAsync();
    Task<TemplateDto> CreateAsync(Guid companyId, CreateTemplateDto dto);
    Task UpdateAsync(Guid id, UpdateTemplateDto dto);
    Task DeleteAsync(Guid id);
}
