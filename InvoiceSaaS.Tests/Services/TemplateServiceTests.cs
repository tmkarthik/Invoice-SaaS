using InvoiceSaaS.Application.DTOs.Template;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using InvoiceSaaS.Domain.Entities;
using Moq;

namespace InvoiceSaaS.Tests.Services;

public class TemplateServiceTests
{
    private readonly Mock<IGenericRepository<Template>> _templateRepositoryMock;
    private readonly Mock<ITenantProvider> _tenantProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TemplateService _templateService;
    private readonly Guid _tenantId = Guid.NewGuid();

    public TemplateServiceTests()
    {
        _templateRepositoryMock = new Mock<IGenericRepository<Template>>();
        _tenantProviderMock = new Mock<ITenantProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _tenantProviderMock.Setup(x => x.GetTenantId()).Returns(_tenantId);
        
        _templateService = new TemplateService(_templateRepositoryMock.Object, _tenantProviderMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTemplateDoesNotExist()
    {
        var templateId = Guid.NewGuid();
        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync((Template?)null);

        var result = await _templateService.GetByIdAsync(templateId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTemplate_WhenTemplateExists()
    {
        var templateId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var template = new Template(_tenantId, companyId, "Invoice Template 1", "{}", true);
        
        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync(template);

        var result = await _templateService.GetByIdAsync(templateId);

        Assert.NotNull(result);
        Assert.Equal("Invoice Template 1", result.Name);
        Assert.Equal("{}", result.TemplateJson);
        Assert.True(result.IsDefault);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTemplateDto()
    {
        var companyId = Guid.NewGuid();
        var createDto = new CreateTemplateDto("New Template", "{\"key\":\"value\"}", false);

        _templateRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Template>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _templateService.CreateAsync(companyId, createDto);

        Assert.NotNull(result);
        Assert.Equal("New Template", result.Name);
        _templateRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Template>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithIsDefault_ShouldUnsetDefaultTemplates()
    {
        var companyId = Guid.NewGuid();
        var existingTemplate = new Template(_tenantId, companyId, "Old Default", "{}", true);
        
        var templates = new List<Template> { existingTemplate }.AsQueryable();
        _templateRepositoryMock.Setup(x => x.GetQueryable()).Returns(templates);

        var createDto = new CreateTemplateDto("New Default", "{}", true);

        await _templateService.CreateAsync(companyId, createDto);

        Assert.False(existingTemplate.IsDefault);
        _templateRepositoryMock.Verify(x => x.Update(existingTemplate), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenTemplateNotFound()
    {
        var templateId = Guid.NewGuid();
        var updateDto = new UpdateTemplateDto("Updated Name", "{}", false);

        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync((Template?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _templateService.UpdateAsync(templateId, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTemplate()
    {
        var templateId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var template = new Template(_tenantId, companyId, "Old Name", "{}");
        var updateDto = new UpdateTemplateDto("New Name", "{\"updated\":true}", false);

        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync(template);

        await _templateService.UpdateAsync(templateId, updateDto);

        Assert.Equal("New Name", template.Name);
        Assert.Equal("{\"updated\":true}", template.TemplateJson);
        Assert.False(template.IsDefault);
        _templateRepositoryMock.Verify(x => x.Update(template), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenTemplateNotFound()
    {
        var templateId = Guid.NewGuid();
        
        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync((Template?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _templateService.DeleteAsync(templateId));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTemplate()
    {
        var templateId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var template = new Template(_tenantId, companyId, "Delete Me", "{}");

        _templateRepositoryMock.Setup(x => x.GetByIdAsync(templateId)).ReturnsAsync(template);

        await _templateService.DeleteAsync(templateId);

        _templateRepositoryMock.Verify(x => x.Delete(template), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
