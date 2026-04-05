using InvoiceSaaS.Application.DTOs;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Application.Services;

public sealed class DashboardService(
    IInvoiceRepository invoiceRepository,
    IActivityLogRepository activityLogRepository) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardSummaryAsync(Guid tenantId)
    {
        var invoices = await invoiceRepository.GetAllByTenantAsync(tenantId);
        var recentLogs = await activityLogRepository.GetRecentByTenantAsync(tenantId, 5);

        var now = DateTime.UtcNow;

        return new DashboardDto
        {
            TotalRevenue = invoices
                .Where(x => x.Status == InvoiceStatus.Paid)
                .Sum(x => x.Amount),

            OverdueAmount = invoices
                .Where(x => x.Status != InvoiceStatus.Paid && x.DueDateUtc < now)
                .Sum(x => x.Amount),

            ActiveInvoicesCount = invoices
                .Count(x => x.Status != InvoiceStatus.Paid && x.Status != InvoiceStatus.Cancelled),

            OverdueInvoicesCount = invoices
                .Count(x => x.Status != InvoiceStatus.Paid && x.DueDateUtc < now),

            UnpaidAmount = invoices
                .Where(x => x.Status != InvoiceStatus.Paid && x.Status != InvoiceStatus.Cancelled)
                .Sum(x => x.Amount),

            RecentActivities = recentLogs.Select(x => new RecentActivityDto
            {
                Action = x.Action,
                Details = x.Details,
                Timestamp = x.CreatedAtUtc
            }).ToList()
        };
    }
}
