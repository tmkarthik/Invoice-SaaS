using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Domain.Enums;

namespace InvoiceSaaS.Infrastructure.Repositories;

public sealed class InMemoryInvoiceRepository : IInvoiceRepository
{
    private static readonly Guid DefaultTenant = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DefaultCompany = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly List<Invoice> Invoices =
    [
        SeedInvoice()
    ];

    static InMemoryInvoiceRepository()
    {
        Invoices[0].SetTenant(DefaultTenant);
    }

    private static Invoice SeedInvoice()
    {
        var customer = new Customer(DefaultTenant, DefaultCompany, "Acme Corp", "ap@acme.com");

        var product = new Product(DefaultTenant, DefaultCompany, "SaaS Subscription", 1200.00m, "SUBS-001");

        var invoice = new Invoice(
            DefaultTenant,
            DefaultCompany,
            customer.Id,
            "INV-0001",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(15),
            "USD");

        invoice.SetStatus(InvoiceStatus.Sent);
        invoice.AddItem(new InvoiceItem(invoice.Id, product.Id, "Monthly plan", 1, 1200.00m));
        return invoice;
    }

    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Invoices.FirstOrDefault(x => x.Id == id));
    }

    public Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Invoice>>(Invoices);
    }

    public IQueryable<Invoice> GetQueryable()
    {
        return Invoices.AsQueryable();
    }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        Invoices.Add(invoice);
        return Task.CompletedTask;
    }

    public void Update(Invoice invoice)
    {
        var existing = Invoices.FirstOrDefault(x => x.Id == invoice.Id);
        if (existing != null)
        {
            Invoices.Remove(existing);
            Invoices.Add(invoice);
        }
    }

    public void Delete(Invoice invoice)
    {
        Invoices.Remove(invoice);
    }

    public Task<IReadOnlyCollection<Invoice>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var result = Invoices
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<Invoice>>(result);
    }
}
