using System.Reflection;
using InvoiceSaaS.Domain.Common;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Persistence;

public sealed class InvoiceSaaSDbContext(
    DbContextOptions<InvoiceSaaSDbContext> options,
    ITenantProvider tenantProvider) : DbContext(options)
{
    private readonly ITenantProvider _tenantProvider = tenantProvider;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
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
        var tenantId = _tenantProvider.GetTenantId();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetTenant(tenantId);
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
}
