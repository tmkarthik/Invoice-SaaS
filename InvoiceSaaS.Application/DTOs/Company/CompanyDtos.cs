namespace InvoiceSaaS.Application.DTOs.Company;

public record CompanyDto(
    Guid Id,
    Guid TenantId,
    string LegalName,
    string? GstNumber, // User called it GSTNumber in prompt, TaxNumber in entity. I'll use GstNumber as requested.
    string Email,
    string? Phone,
    string Currency,
    string TimeZone);

public record CreateCompanyRequest(
    Guid TenantId,
    string LegalName,
    string? GstNumber,
    string Email,
    string? Phone,
    string Currency = "INR",
    string TimeZone = "Asia/Kolkata");
