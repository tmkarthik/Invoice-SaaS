using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Infrastructure.Repositories;
using Moq;

namespace InvoiceSaaS.Tests.Application;

public sealed class InvoiceServiceTests
{
    private static readonly Guid DefaultTenant = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task GetInvoicesAsync_ReturnsSeededInvoice_ForDefaultTenant()
    {
        var repository = new InMemoryInvoiceRepository();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var customerRepoMock = new Mock<IGenericRepository<Customer>>();
        var companyRepoMock = new Mock<IGenericRepository<Company>>();
        var tenantProviderMock = new Mock<ITenantProvider>();

        tenantProviderMock.Setup(x => x.GetTenantId()).Returns(DefaultTenant);

        var service = new InvoiceService(
            repository,
            customerRepoMock.Object,
            companyRepoMock.Object,
            tenantProviderMock.Object,
            unitOfWorkMock.Object);

        var result = await service.GetInvoicesAsync();

        Assert.NotEmpty(result);
    }
}
