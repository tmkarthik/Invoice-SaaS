using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardSummaryAsync(Guid tenantId);
}
