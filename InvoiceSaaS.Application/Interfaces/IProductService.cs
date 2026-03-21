using InvoiceSaaS.Application.DTOs.Product;

namespace InvoiceSaaS.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm);
    Task<ProductDto> CreateAsync(Guid companyId, CreateProductDto dto);
    Task UpdateAsync(Guid id, UpdateProductDto dto);
    Task DeleteAsync(Guid id);
}
