using InvoiceSaaS.Application.DTOs.Customer;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class CustomerService(
    IGenericRepository<Customer> customerRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public Task<(IEnumerable<CustomerDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm)
    {
        var query = customerRepository.GetQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(c => c.DisplayName.ToLower().Contains(term) || c.Email.ToLower().Contains(term));
        }

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IEnumerable<CustomerDto> Items, int TotalCount)>((items.Select(MapToDto), totalCount));
    }

    public async Task<CustomerDto> CreateAsync(Guid companyId, CreateCustomerDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        var customer = new Customer(tenantId, companyId, dto.DisplayName, dto.Email, dto.GstNumber);
        await customerRepository.AddAsync(customer);
        await unitOfWork.SaveChangesAsync();
        
        return MapToDto(customer);
    }

    public async Task UpdateAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await customerRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found.");

        customer.UpdateDetails(dto.DisplayName, dto.Email, dto.GstNumber);
        
        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await customerRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found.");

        customerRepository.Delete(customer);
        await unitOfWork.SaveChangesAsync();
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto(customer.Id, customer.DisplayName, customer.Email, customer.GstNumber);
    }
}
