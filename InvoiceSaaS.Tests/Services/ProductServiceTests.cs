using InvoiceSaaS.Application.DTOs.Product;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using InvoiceSaaS.Domain.Entities;
using Moq;

namespace InvoiceSaaS.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IGenericRepository<Product>> _productRepositoryMock;
    private readonly Mock<IGenericRepository<Company>> _companyRepositoryMock;
    private readonly Mock<ITenantProvider> _tenantProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IGenericRepository<Product>>();
        _companyRepositoryMock = new Mock<IGenericRepository<Company>>();
        _tenantProviderMock = new Mock<ITenantProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _tenantProviderMock.Setup(x => x.GetTenantId()).Returns(_tenantId);
        _tenantProviderMock.Setup(x => x.IsAdmin).Returns(false);

        _productService = new ProductService(
            _productRepositoryMock.Object,
            _companyRepositoryMock.Object,
            _tenantProviderMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var result = await _productService.GetByIdAsync(productId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var product = new Product(_tenantId, companyId, "Test Product", 10.5m, "SKU123", "Description", 5m);

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(productId);

        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(10.5m, result.UnitPrice);
        Assert.Equal("SKU123", result.Sku);
        Assert.Equal("Description", result.Description);
        Assert.Equal(5m, result.TaxPercent);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnProductDto()
    {
        var companyId = Guid.NewGuid();
        var company = new Company(_tenantId, "Test Co", "GST", "co@test.com", null, "INR", "Asia/Kolkata");
        var createDto = new CreateProductDto(_tenantId, companyId, "New Product", 20m, "SKU456", "New Desc", 10m);

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId)).ReturnsAsync(company);
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _productService.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto("Updated Name", 15m, "UPSKU", "UPDesc", 15m);

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.UpdateAsync(productId, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        var productId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var product = new Product(_tenantId, companyId, "Old Name", 10m, "OLDSKU");
        var updateDto = new UpdateProductDto("New Name", 15m, "NEWSKU", "NewDesc", 5m);

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        await _productService.UpdateAsync(productId, updateDto);

        Assert.Equal("New Name", product.Name);
        Assert.Equal(15m, product.UnitPrice);
        Assert.Equal("NEWSKU", product.Sku);
        Assert.Equal("NewDesc", product.Description);
        Assert.Equal(5m, product.TaxPercent);
        _productRepositoryMock.Verify(x => x.Update(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = Guid.NewGuid();

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.DeleteAsync(productId));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        var productId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var product = new Product(_tenantId, companyId, "Delete Me", 10m, "DELSKU");

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        await _productService.DeleteAsync(productId);

        _productRepositoryMock.Verify(x => x.Delete(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
