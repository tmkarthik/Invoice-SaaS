namespace InvoiceSaaS.Application.DTOs.Product;

public record ProductDto(Guid Id, string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
public record CreateProductDto(string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
public record UpdateProductDto(string Name, decimal UnitPrice, string Sku, string? Description, decimal TaxPercent);
