using InvoiceSaaS.Application.DTOs.Tenant;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Application.Services;

public sealed class TenantService(
    IGenericRepository<Tenant> tenantRepository,
    IGenericRepository<Company> companyRepository,
    IGenericRepository<User> userRepository,
    IGenericRepository<InvoiceSettings> settingsRepository,
    IGenericRepository<Template> templateRepository,
    IGenericRepository<RefreshToken> refreshTokenRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : ITenantService
{
    public async Task<TenantDto> CreateTenantAsync(CreateTenantRequest request)
    {
        var tenant = new Tenant(request.Name, request.Email, request.Phone);
        await tenantRepository.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        return MapToDto(tenant);
    }

    public async Task<TenantDto> GetTenantAsync(Guid tenantId)
    {
        var tenant = await tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException($"Tenant {tenantId} not found.");
        return MapToDto(tenant);
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        var tenants = await tenantRepository.GetAllAsync();
        return tenants.Select(MapToDto);
    }

    public async Task UpgradeTenantAsync(Guid tenantId, UpgradeTenantRequest request)
    {
        var tenant = await tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException($"Tenant {tenantId} not found.");

        tenant.UpgradePlan(request.PlanName, request.MaxUsers, request.MaxInvoices);
        tenantRepository.Update(tenant);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateTenantAsync(Guid tenantId)
    {
        var tenant = await tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException($"Tenant {tenantId} not found.");

        tenant.Deactivate();
        tenantRepository.Update(tenant);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<RegisterTenantResponse> RegisterTenantAsync(CreateTenantRequest request)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Create Tenant
            var tenant = new Tenant(request.Name, request.Email, request.Phone);
            await tenantRepository.AddAsync(tenant);
            await unitOfWork.SaveChangesAsync();

            // 2. Create default Company
            var company = new Company(tenant.Id, $"{request.Name} Core", "GST-PENDING", request.Email, request.Phone);
            await companyRepository.AddAsync(company);
            await unitOfWork.SaveChangesAsync();

            // 3. Create Admin user
            var admin = new User(tenant.Id, company.Id, request.Email, "Admin");
            admin.SetPassword(passwordHasher.HashPassword("DefaultPassword123!")); // Production should handle this better
            admin.SetRole("Admin");
            await userRepository.AddAsync(admin);
            await unitOfWork.SaveChangesAsync();

            // 4. Create default InvoiceSettings
            var settings = new InvoiceSettings(tenant.Id, company.Id, "INV-", 1000);
            await settingsRepository.AddAsync(settings);

            // 5. Create default Template
            var template = new Template(tenant.Id, company.Id, "Default Blue", "{\"layout\":\"modern\",\"color\":\"#0000FF\"}", true);
            await templateRepository.AddAsync(template);

            await unitOfWork.SaveChangesAsync();

            // 6. Generate Tokens
            var authResult = await GenerateTokens(admin);

            await unitOfWork.CommitTransactionAsync();

            return new RegisterTenantResponse(MapToDto(tenant), authResult);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task<AuthResult> GenerateTokens(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new AuthResult(accessToken, refreshToken.Token);
    }

    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto(
            tenant.Id,
            tenant.Name,
            tenant.Email,
            tenant.Phone,
            tenant.PlanName,
            tenant.IsActive,
            tenant.MaxUsers,
            tenant.MaxInvoices);
    }
}
