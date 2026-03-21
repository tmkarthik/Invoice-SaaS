using InvoiceSaaS.Application.DTOs.Template;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class TemplateService(
    IGenericRepository<Template> templateRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : ITemplateService
{
    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        var template = await templateRepository.GetByIdAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<IEnumerable<TemplateDto>> GetAllAsync()
    {
        var templates = await templateRepository.GetAllAsync();
        return templates.Select(MapToDto);
    }

    public async Task<TemplateDto> CreateAsync(Guid companyId, CreateTemplateDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        var template = new Template(tenantId, companyId, dto.Name, dto.TemplateJson, dto.IsDefault);
        
        if (dto.IsDefault)
        {
            await UnsetDefaultTemplatesAsync();
        }

        await templateRepository.AddAsync(template);
        await unitOfWork.SaveChangesAsync();
        
        return MapToDto(template);
    }

    public async Task UpdateAsync(Guid id, UpdateTemplateDto dto)
    {
        var template = await templateRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Template with ID {id} not found.");

        if (dto.IsDefault && !template.IsDefault)
        {
            await UnsetDefaultTemplatesAsync();
        }

        template.UpdateDetails(dto.Name, dto.TemplateJson, dto.IsDefault);
        
        templateRepository.Update(template);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var template = await templateRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Template with ID {id} not found.");

        templateRepository.Delete(template);
        await unitOfWork.SaveChangesAsync();
    }
    
    private async Task UnsetDefaultTemplatesAsync()
    {
       var query = templateRepository.GetQueryable().Where(t => t.IsDefault);
       var defaults = query.ToList();
       foreach(var def in defaults)
       {
           def.UpdateDetails(def.Name, def.TemplateJson, false);
           templateRepository.Update(def);
       }
       await Task.CompletedTask;
    }

    private static TemplateDto MapToDto(Template template)
    {
        return new TemplateDto(template.Id, template.Name, template.TemplateJson, template.IsDefault);
    }
}
