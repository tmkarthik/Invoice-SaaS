namespace InvoiceSaaS.Application.DTOs;

public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public required string InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
}
