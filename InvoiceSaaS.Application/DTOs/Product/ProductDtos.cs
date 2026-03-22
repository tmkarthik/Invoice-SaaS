namespace InvoiceSaaS.Application.DTOs.Product;

public record ProductDto(Guid Id, Guid TenantId, Guid CompanyId, string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
public record CreateProductDto(Guid TenantId, Guid CompanyId, string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
public record UpdateProductDto(string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
