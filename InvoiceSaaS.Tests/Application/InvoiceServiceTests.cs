using InvoiceSaaS.Application.Services;
using InvoiceSaaS.Infrastructure.Repositories;

namespace InvoiceSaaS.Tests.Application;

public sealed class InvoiceServiceTests
{
    private static readonly Guid DefaultTenant = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task GetInvoicesAsync_ReturnsSeededInvoice_ForDefaultTenant()
    {
        var repository = new InMemoryInvoiceRepository();
        var unitOfWorkMock = new Moq.Mock<InvoiceSaaS.Application.Interfaces.IUnitOfWork>();
        var service = new InvoiceService(repository, unitOfWorkMock.Object);

        var result = await service.GetInvoicesAsync(DefaultTenant);

        Assert.NotEmpty(result);
    }
}
