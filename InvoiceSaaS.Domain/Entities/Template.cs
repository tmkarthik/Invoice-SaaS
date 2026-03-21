using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class Template : BaseEntity
{
    public Template(Guid companyId, string name, string content, bool isDefault = false)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Template name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Template content is required.", nameof(content));

        CompanyId = companyId;
        Name = name.Trim();
        Content = content;
        IsDefault = isDefault;
    }

    private Template()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
}
