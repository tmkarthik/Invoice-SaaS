namespace InvoiceSaaS.Application.DTOs;

public sealed class DashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal OverdueAmount { get; set; }
    public int ActiveInvoicesCount { get; set; }
    public int OverdueInvoicesCount { get; set; }
    public decimal UnpaidAmount { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = [];
}

public sealed class RecentActivityDto
{
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
