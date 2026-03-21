using InvoiceSaaS.Domain.Common;

namespace InvoiceSaaS.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public RefreshToken(Guid userId, string token, DateTime expiryDateUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token is required.", nameof(token));
        
        UserId = userId;
        Token = token.Trim();
        ExpiryDateUtc = expiryDateUtc;
    }

    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiryDateUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    public void Revoke()
    {
        IsRevoked = true;
        Touch();
    }
}
