namespace InvoiceSaaS.Application.DTOs;

public record AddInvoiceItemDto(Guid ProductId, string Description, decimal Quantity, decimal UnitPrice, decimal TaxRate);
public record CreateInvoiceDto(Guid CompanyId, Guid CustomerId, string Number, DateTime IssueDateUtc, DateTime DueDateUtc, string Currency, decimal Discount, List<AddInvoiceItemDto> Items);
