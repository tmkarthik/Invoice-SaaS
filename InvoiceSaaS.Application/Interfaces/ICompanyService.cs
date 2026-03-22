using InvoiceSaaS.Application.DTOs.Company;

namespace InvoiceSaaS.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request);
    Task<CompanyDto> GetCompanyAsync(Guid companyId);
    Task<IEnumerable<CompanyDto>> GetCompaniesByTenantAsync(Guid tenantId);
}
