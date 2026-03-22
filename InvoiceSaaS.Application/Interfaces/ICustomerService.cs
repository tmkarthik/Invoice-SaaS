using InvoiceSaaS.Application.DTOs.Customer;

namespace InvoiceSaaS.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<IEnumerable<CustomerDto>> GetByCompanyAsync(Guid companyId);
    Task<(IEnumerable<CustomerDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task UpdateAsync(Guid id, UpdateCustomerDto dto);
    Task DeleteAsync(Guid id);
}
