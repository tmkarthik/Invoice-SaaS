using InvoiceSaaS.Application.DTOs.Customer;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class CustomerService(
    IGenericRepository<Customer> customerRepository,
    IGenericRepository<Company> companyRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var tenantId = tenantProvider.GetTenantId();
        var customer = await customerRepository.GetByIdAsync(id);
        
        if (customer == null || (customer.TenantId != tenantId && !tenantProvider.IsAdmin()))
            return null;

        return MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var tenantId = tenantProvider.GetTenantId();
        var customers = await Task.FromResult(customerRepository.GetQueryable()
            .Where(x => x.TenantId == tenantId)
            .ToList());
            
        return customers.Select(MapToDto);
    }

    public async Task<IEnumerable<CustomerDto>> GetByCompanyAsync(Guid companyId)
    {
        var tenantId = tenantProvider.GetTenantId();
        var customers = await Task.FromResult(customerRepository.GetQueryable()
            .Where(x => x.TenantId == tenantId && x.CompanyId == companyId)
            .ToList());

        return customers.Select(MapToDto);
    }

    public Task<(IEnumerable<CustomerDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm)
    {
        var tenantId = tenantProvider.GetTenantId();
        var query = customerRepository.GetQueryable().Where(x => x.TenantId == tenantId);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(c => c.Name.ToLower().Contains(term) || c.Email.ToLower().Contains(term));
        }

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IEnumerable<CustomerDto> Items, int TotalCount)>((items.Select(MapToDto), totalCount));
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        
        // Step 8: Validation: Customer TenantId must equal Company TenantId
        var company = await companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new KeyNotFoundException($"Company {dto.CompanyId} not found.");
        if (company.TenantId != dto.TenantId) throw new InvalidOperationException("Company TenantId mismatch.");

        // Prevent cross-tenant creation
        if (dto.TenantId != tenantId && !tenantProvider.IsAdmin())
        {
            throw new UnauthorizedAccessException("Cannot create customer for another tenant.");
        }

        var customer = new Customer(dto.TenantId, dto.CompanyId, dto.Name, dto.Email, dto.Phone, dto.GstNumber);
        
        await customerRepository.AddAsync(customer);
        await unitOfWork.SaveChangesAsync();
        
        return MapToDto(customer);
    }

    public async Task UpdateAsync(Guid id, UpdateCustomerDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        var customer = await customerRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found.");

        if (customer.TenantId != tenantId && !tenantProvider.IsAdmin())
            throw new UnauthorizedAccessException("Access denied.");

        customer.UpdateDetails(dto.Name, dto.Email, dto.Phone, dto.GstNumber);
        
        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenantId = tenantProvider.GetTenantId();
        var customer = await customerRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found.");

        if (customer.TenantId != tenantId && !tenantProvider.IsAdmin())
            throw new UnauthorizedAccessException("Access denied.");

        customerRepository.Delete(customer);
        await unitOfWork.SaveChangesAsync();
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto(
            customer.Id, 
            customer.TenantId, 
            customer.CompanyId, 
            customer.Name, 
            customer.Email, 
            customer.Phone ?? string.Empty,
            customer.GstNumber);
    }
}
