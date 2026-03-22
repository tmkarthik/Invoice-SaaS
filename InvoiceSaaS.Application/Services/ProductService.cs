using InvoiceSaaS.Application.DTOs.Product;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class ProductService(
    IGenericRepository<Product> productRepository,
    IGenericRepository<Company> companyRepository,
    ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var tenantId = tenantProvider.GetTenantId();
        var product = await productRepository.GetByIdAsync(id);
        
        if (product == null || (product.TenantId != tenantId && !tenantProvider.IsAdmin()))
            return null;

        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var tenantId = tenantProvider.GetTenantId();
        var products = await Task.FromResult(productRepository.GetQueryable()
            .Where(x => x.TenantId == tenantId)
            .ToList());
            
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByCompanyAsync(Guid companyId)
    {
        var tenantId = tenantProvider.GetTenantId();
        var products = await Task.FromResult(productRepository.GetQueryable()
            .Where(x => x.TenantId == tenantId && x.CompanyId == companyId)
            .ToList());

        return products.Select(MapToDto);
    }

    public Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm)
    {
        var tenantId = tenantProvider.GetTenantId();
        var query = productRepository.GetQueryable().Where(x => x.TenantId == tenantId);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Sku.ToLower().Contains(term));
        }

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IEnumerable<ProductDto> Items, int TotalCount)>((items.Select(MapToDto), totalCount));
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();

        // Step 9: Validate Company TenantId matches Product TenantId
        var company = await companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null) throw new KeyNotFoundException($"Company {dto.CompanyId} not found.");
        if (company.TenantId != dto.TenantId) throw new InvalidOperationException("Company TenantId mismatch.");

        // Prevent cross-tenant creation
        if (dto.TenantId != tenantId && !tenantProvider.IsAdmin())
        {
            throw new UnauthorizedAccessException("Cannot create product for another tenant.");
        }

        var product = new Product(dto.TenantId, dto.CompanyId, dto.Name, dto.UnitPrice, dto.Sku, dto.Description, dto.TaxPercent);
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();
        
        return MapToDto(product);
    }

    public async Task UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var tenantId = tenantProvider.GetTenantId();
        var product = await productRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        if (product.TenantId != tenantId && !tenantProvider.IsAdmin())
            throw new UnauthorizedAccessException("Access denied.");

        product.UpdateDetails(dto.Name, dto.UnitPrice, dto.Sku, dto.Description, dto.TaxPercent);
        
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenantId = tenantProvider.GetTenantId();
        var product = await productRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        if (product.TenantId != tenantId && !tenantProvider.IsAdmin())
            throw new UnauthorizedAccessException("Access denied.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id, 
            product.TenantId, 
            product.CompanyId, 
            product.Name, 
            product.UnitPrice, 
            product.Sku, 
            product.Description, 
            product.TaxPercent);
    }
}
