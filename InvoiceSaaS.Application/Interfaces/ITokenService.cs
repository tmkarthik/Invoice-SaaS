using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid tenantId, Guid userId);
}
