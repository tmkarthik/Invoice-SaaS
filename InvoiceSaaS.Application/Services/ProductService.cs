using InvoiceSaaS.Application.DTOs.Product;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class ProductService(
    IGenericRepository<Product> productRepository,
    IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm)
    {
        var query = productRepository.GetQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Sku.ToLower().Contains(term));
        }

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IEnumerable<ProductDto> Items, int TotalCount)>((items.Select(MapToDto), totalCount));
    }

    public async Task<ProductDto> CreateAsync(Guid companyId, CreateProductDto dto)
    {
        var product = new Product(companyId, dto.Name, dto.UnitPrice, dto.Sku, dto.Description, dto.TaxPercent);
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();
        
        return MapToDto(product);
    }

    public async Task UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await productRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        product.UpdateDetails(dto.Name, dto.UnitPrice, dto.Sku, dto.Description, dto.TaxPercent);
        
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(product.Id, product.Name, product.UnitPrice, product.Sku, product.Description, product.TaxPercent);
    }
}
