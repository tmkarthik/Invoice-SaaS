using InvoiceSaaS.Domain.Entities;

namespace InvoiceSaaS.Tests.Domain;

public class InvoiceLogicTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _companyId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();

    [Fact]
    public void AddItem_WithZeroTax_CalculatesCorrectly()
    {
        var invoice = new Invoice(_tenantId, _companyId, _customerId, "INV-001", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "USD");
        
        var item = new InvoiceItem(invoice.Id, Guid.NewGuid(), "Item 1", 2, 100m, 0m);
        
        invoice.AddItem(item);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.Equal(0m, invoice.TotalTax);
        Assert.Equal(0m, invoice.Discount);
        Assert.Equal(200m, invoice.Amount);
    }

    [Fact]
    public void AddItem_WithMultipleTaxes_CalculatesCorrectly()
    {
        var invoice = new Invoice(_tenantId, _companyId, _customerId, "INV-002", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "USD");
        
        var item1 = new InvoiceItem(invoice.Id, Guid.NewGuid(), "Item 1", 1, 100m, 0.10m); 
        var item2 = new InvoiceItem(invoice.Id, Guid.NewGuid(), "Item 2", 2, 50m, 0.05m);  
        
        invoice.AddItem(item1);
        invoice.AddItem(item2);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.Equal(15m, invoice.TotalTax);  
        Assert.Equal(215m, invoice.Amount);   
    }

    [Fact]
    public void SetDiscount_DiscountGreaterThanSubtotal_AmountIsZero()
    {
        var invoice = new Invoice(_tenantId, _companyId, _customerId, "INV-003", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "USD");
        
        var item = new InvoiceItem(invoice.Id, Guid.NewGuid(), "Item 1", 1, 100m, 0m);
        invoice.AddItem(item);
        
        Assert.Equal(100m, invoice.Subtotal);
        Assert.Equal(100m, invoice.Amount);
        
        invoice.SetDiscount(150m);
        
        Assert.Equal(100m, invoice.Subtotal);
        Assert.Equal(150m, invoice.Discount);
        Assert.Equal(0m, invoice.Amount); 
    }

    [Fact]
    public void SetDiscount_ValidNumber_CalculatesCorrectly()
    {
        var invoice = new Invoice(_tenantId, _companyId, _customerId, "INV-004", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "USD");
        
        var item = new InvoiceItem(invoice.Id, Guid.NewGuid(), "Item 1", 1, 100m, 0.10m); 
        invoice.AddItem(item);
        invoice.SetDiscount(20m);
        
        Assert.Equal(100m, invoice.Subtotal);
        Assert.Equal(10m, invoice.TotalTax);
        Assert.Equal(20m, invoice.Discount);
        Assert.Equal(90m, invoice.Amount);
    }
}
