namespace InvoiceSaaS.Application.DTOs.Tenant;

public record TenantDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string PlanName,
    bool IsActive,
    int MaxUsers,
    int MaxInvoices);

public record CreateTenantRequest(
    string Name,
    string Email,
    string? Phone);

public record UpgradeTenantRequest(
    string PlanName,
    int MaxUsers,
    int MaxInvoices);
