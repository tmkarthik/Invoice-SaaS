using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Application.Services;

public sealed class AuthService(
    IGenericRepository<User> userRepository,
    IGenericRepository<RefreshToken> refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(Guid tenantId, Guid companyId, string email, string fullName, string password)
    {
        var existingUser = await userRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
            
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User(tenantId, companyId, email, fullName);
        user.SetPassword(passwordHasher.HashPassword(password));
        
        await userRepository.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        return await GenerateTokens(user);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await userRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null || !user.IsActive || !passwordHasher.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await GenerateTokens(user);
    }

    public async Task<AuthResult> RefreshTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDateUtc <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("User not found or inactive.");

        refreshToken.Revoke();
        refreshTokenRepository.Update(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return await GenerateTokens(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var activeTokens = await refreshTokenRepository.GetQueryable()
            .IgnoreQueryFilters()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var rt in activeTokens)
        {
            rt.Revoke();
            refreshTokenRepository.Update(rt);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private async Task<AuthResult> GenerateTokens(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.TenantId, user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new AuthResult(accessToken, refreshToken.Token);
    }
}
