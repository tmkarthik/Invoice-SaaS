using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Tenant : BaseAuditableEntity
{
    private readonly List<Company> _companies = [];
    private readonly List<User> _users = [];

    public Tenant(string name, string email, string planName = "Free")
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tenant name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(planName)) throw new ArgumentException("Plan name is required.", nameof(planName));

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PlanName = planName.Trim();
        IsActive = true;
    }

    private Tenant()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string PlanName { get; private set; } = "Free";
    public bool IsActive { get; private set; }
    public int MaxUsers { get; private set; }
    public int MaxInvoices { get; private set; }

    public IReadOnlyCollection<Company> Companies => _companies.AsReadOnly();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void UpgradePlan(string plan, int maxUsers, int maxInvoices)
    {
        if (string.IsNullOrWhiteSpace(plan)) throw new ArgumentException("Plan name is required.", nameof(plan));
        
        PlanName = plan.Trim();
        MaxUsers = maxUsers;
        MaxInvoices = maxInvoices;
        Touch();
    }
}
