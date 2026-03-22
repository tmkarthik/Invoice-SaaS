using InvoiceSaaS.Application.DTOs.Company;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class CompanyService(
    IGenericRepository<Company> companyRepository,
    IGenericRepository<Tenant> tenantRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : ICompanyService
{
    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request)
    {
        // Step 7: Validate Tenant exists
        var tenant = await tenantRepository.GetByIdAsync(request.TenantId);
        if (tenant == null) throw new KeyNotFoundException($"Tenant {request.TenantId} not found.");

        // Step 7: Ensure Company TenantId must match logged tenant
        var currentTenantId = tenantProvider.GetTenantId();
        if (request.TenantId != currentTenantId && !tenantProvider.IsAdmin)
        {
            throw new UnauthorizedAccessException("Cannot create company for another tenant.");
        }

        var company = new Company(
            request.TenantId, 
            request.LegalName, 
            request.GstNumber ?? "PENDING",
            request.Email,
            request.Phone,
            request.Currency,
            request.TimeZone);

        await companyRepository.AddAsync(company);
        await unitOfWork.SaveChangesAsync();

        return MapToDto(company);
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid companyId)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        if (company == null) throw new KeyNotFoundException($"Company {companyId} not found.");

        // Security check
        var currentTenantId = tenantProvider.GetTenantId();
        if (company.TenantId != currentTenantId && !tenantProvider.IsAdmin)
        {
            throw new UnauthorizedAccessException("Access denied to this company.");
        }

        return MapToDto(company);
    }

    public async Task<IEnumerable<CompanyDto>> GetCompaniesByTenantAsync(Guid tenantId)
    {
        // Security check
        var currentTenantId = tenantProvider.GetTenantId();
        if (tenantId != currentTenantId && !tenantProvider.IsAdmin)
        {
            throw new UnauthorizedAccessException("Access denied to these companies.");
        }

        var companies = await companyRepository.GetAllAsync();
        return companies
            .Where(x => x.TenantId == tenantId)
            .Select(MapToDto);
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto(
            company.Id,
            company.TenantId,
            company.LegalName,
            company.TaxNumber,
            company.Email,
            company.Phone,
            company.Currency,
            company.TimeZone
        );
    }
}
