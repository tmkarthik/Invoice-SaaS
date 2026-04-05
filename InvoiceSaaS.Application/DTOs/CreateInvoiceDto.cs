namespace InvoiceSaaS.Application.DTOs;

public record AddInvoiceItemDto(Guid ProductId, string Description, decimal Quantity, decimal UnitPrice, List<Guid> TaxIds);
public record CreateInvoiceDto(Guid TenantId, Guid CompanyId, Guid CustomerId, string Number, DateTime IssueDate, DateTime DueDate, string Currency, decimal Discount, List<AddInvoiceItemDto> Items);
