using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Persistence;

public sealed class InvoiceSaaSDbContext(DbContextOptions<InvoiceSaaSDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Template> Templates => Set<Template>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceSaaSDbContext).Assembly);
    }
}
