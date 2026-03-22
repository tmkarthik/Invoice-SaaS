using InvoiceSaaS.Application.DTOs.Customer;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using InvoiceSaaS.Domain.Entities;
using Moq;

namespace InvoiceSaaS.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<IGenericRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IGenericRepository<Company>> _companyRepositoryMock;
    private readonly Mock<ITenantProvider> _tenantProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CustomerService _customerService;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<IGenericRepository<Customer>>();
        _companyRepositoryMock = new Mock<IGenericRepository<Company>>();
        _tenantProviderMock = new Mock<ITenantProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _tenantProviderMock.Setup(x => x.GetTenantId()).Returns(_tenantId);
        _tenantProviderMock.Setup(x => x.IsAdmin).Returns(false);

        _customerService = new CustomerService(
            _customerRepositoryMock.Object,
            _companyRepositoryMock.Object,
            _tenantProviderMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        var customerId = Guid.NewGuid();
        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

        var result = await _customerService.GetByIdAsync(customerId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
    {
        var customerId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var customer = new Customer(_tenantId, companyId, "Test Customer", "test@test.com", null, "GST123");

        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

        var result = await _customerService.GetByIdAsync(customerId);

        Assert.NotNull(result);
        Assert.Equal("Test Customer", result.Name);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal("GST123", result.GstNumber);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCustomerDto()
    {
        var companyId = Guid.NewGuid();
        var company = new Company(_tenantId, "Test Co", "GST", "co@test.com", null, "INR", "Asia/Kolkata");
        var createDto = new CreateCustomerDto(_tenantId, companyId, "New Customer", "new@test.com", null, "GST456");

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId)).ReturnsAsync(company);
        _customerRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _customerService.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("New Customer", result.Name);
        _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCustomerNotFound()
    {
        var customerId = Guid.NewGuid();
        var updateDto = new UpdateCustomerDto("Updated Name", "updated@test.com", null, null);

        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.UpdateAsync(customerId, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer()
    {
        var customerId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var customer = new Customer(_tenantId, companyId, "Old Name", "old@test.com");
        var updateDto = new UpdateCustomerDto("New Name", "new@test.com", null, "NEWGST");

        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

        await _customerService.UpdateAsync(customerId, updateDto);

        Assert.Equal("New Name", customer.Name);
        Assert.Equal("new@test.com", customer.Email);
        Assert.Equal("NEWGST", customer.GstNumber);
        _customerRepositoryMock.Verify(x => x.Update(customer), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenCustomerNotFound()
    {
        var customerId = Guid.NewGuid();

        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.DeleteAsync(customerId));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCustomer()
    {
        var customerId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var customer = new Customer(_tenantId, companyId, "Delete Me", "delete@test.com");

        _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

        await _customerService.DeleteAsync(customerId);

        _customerRepositoryMock.Verify(x => x.Delete(customer), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
