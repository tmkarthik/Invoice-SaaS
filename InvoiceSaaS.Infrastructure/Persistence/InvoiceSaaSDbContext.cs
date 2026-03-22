using System.Reflection;
using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Persistence;

public sealed class InvoiceSaaSDbContext(
    DbContextOptions<InvoiceSaaSDbContext> options,
    ITenantProvider tenantProvider) : DbContext(options)
{
    private readonly ITenantProvider _tenantProvider = tenantProvider;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Template> Templates => Set<Template>();

    public Guid CurrentTenantId => _tenantProvider.GetTenantId();
    public bool IsAdmin => _tenantProvider.IsAdmin;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceSaaSDbContext).Assembly);
        modelBuilder.ApplyTenantFilter(this);
    }

    public override int SaveChanges()
    {
        ApplyAuditingRules();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditingRules();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditingRules()
    {
        // Try to get the tenant ID from context — may be null for anonymous flows (e.g. tenant registration)
        var tenantId = TryGetTenantId();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Only stamp the TenantId if the entity doesn't already have one set.
                    // This allows services to set TenantId themselves (e.g. TenantService during onboarding).
                    if (tenantId.HasValue && entry.Entity.TenantId == Guid.Empty)
                    {
                        entry.Entity.SetTenant(tenantId.Value);
                    }
                    entry.Property(x => x.CreatedDate).CurrentValue = DateTime.UtcNow;
                    entry.Property(x => x.UpdatedDate).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.UpdatedDate).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SoftDelete();
                    break;
            }
        }
    }

    /// <summary>
    /// Safely attempts to resolve the current tenant ID without throwing.
    /// Returns null for anonymous/system operations (e.g. tenant onboarding).
    /// </summary>
    private Guid? TryGetTenantId()
    {
        try
        {
            var id = _tenantProvider.GetTenantId();
            return id == Guid.Empty ? null : id;
        }
        catch
        {
            return null;
        }
    }
}
