namespace InvoiceSaaS.Application.DTOs.Customer;

public record CustomerDto(Guid Id, Guid TenantId, Guid CompanyId, string Name, string Email, string? Phone, string? GstNumber);
public record CreateCustomerDto(Guid TenantId, Guid CompanyId, string Name, string Email, string? Phone, string? GstNumber);
public record UpdateCustomerDto(string Name, string Email, string? Phone, string? GstNumber);
