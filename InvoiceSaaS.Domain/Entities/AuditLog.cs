using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class AuditLog : BaseSimpleEntity
{
    public AuditLog(Guid tenantId, Guid userId, string action, string entityName, string entityId)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId is required.", nameof(tenantId));
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action is required.", nameof(action));
        if (string.IsNullOrWhiteSpace(entityName)) throw new ArgumentException("Entity name is required.", nameof(entityName));

        TenantId = tenantId;
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        Timestamp = DateTime.UtcNow;
    }

    private AuditLog() { }

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime Timestamp { get; private set; }

    public void SetDetails(string? oldValues, string? newValues, string? ipAddress)
    {
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
    }
}
