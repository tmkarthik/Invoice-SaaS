namespace InvoiceSaaS.Application.Interfaces;

public sealed record AuthResult(string AccessToken, string RefreshToken);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(Guid companyId, string email, string fullName, string password);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
}
