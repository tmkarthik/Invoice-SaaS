using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class User : BaseEntity
{
    public User(Guid companyId, string email, string fullName)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId cannot be empty.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Full name is required.", nameof(fullName));

        CompanyId = companyId;
        Email = email.Trim().ToLowerInvariant();
        FullName = fullName.Trim();
    }

    private User()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
