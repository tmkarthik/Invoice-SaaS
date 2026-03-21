namespace InvoiceSaaS.Application.DTOs.Customer;

public record CustomerDto(Guid Id, string DisplayName, string Email, string? GstNumber);
public record CreateCustomerDto(string DisplayName, string Email, string? GstNumber);
public record UpdateCustomerDto(string DisplayName, string Email, string? GstNumber);
