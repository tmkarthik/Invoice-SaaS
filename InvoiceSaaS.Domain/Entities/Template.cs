using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Template : BaseEntity
{
    public Template(Guid tenantId, Guid companyId, string name, string templateJson, bool isDefault = false)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Template name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(templateJson)) throw new ArgumentException("Template JSON is required.", nameof(templateJson));

        SetTenant(tenantId);
        CompanyId = companyId;
        Name = name.Trim();
        TemplateJson = templateJson;
        IsDefault = isDefault;
    }

    private Template()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public Tenant? Tenant { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TemplateJson { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }

    public void UpdateDetails(string name, string templateJson, bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Template name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(templateJson)) throw new ArgumentException("Template JSON is required.", nameof(templateJson));

        Name = name.Trim();
        TemplateJson = templateJson;
        IsDefault = isDefault;
        Touch();
    }
}
