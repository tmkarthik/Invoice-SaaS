namespace InvoiceSaaS.Application.DTOs;

public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public CustomerDto? Customer { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public required string InvoiceNumber { get; set; }
    public string Number => InvoiceNumber; // Alias for views
    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TaxRate { get; set; } // Simplified for some views
    public decimal Amount { get; set; }
    public List<TaxBreakdownDto> TaxBreakdown { get; set; } = new();
    public List<InvoiceItemDto> Items { get; set; } = new();
    public List<InvoiceItemDto> InvoiceItems => Items; // Alias for views
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime IssueDateUtc => IssueDate; // Alias for views
    public DateTime DueDateUtc => DueDate; // Alias for views
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
}

public sealed class CustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? BillingAddress { get; set; }
}

public sealed class TaxBreakdownDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

public sealed class InvoiceItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; } // Subtotal
    public decimal Total => Amount + Taxes.Sum(t => t.Amount); // Line Total for views
    public List<TaxBreakdownDto> Taxes { get; set; } = new();
}
