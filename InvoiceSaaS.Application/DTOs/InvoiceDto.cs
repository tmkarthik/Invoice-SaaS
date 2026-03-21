namespace InvoiceSaaS.Application.DTOs;

public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public required string Number { get; set; }
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDateUtc { get; set; }
    public string Status { get; set; } = string.Empty;
}
