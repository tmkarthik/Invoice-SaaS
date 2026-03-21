using InvoiceSaaS.Domain.Entities;
using InvoiceSaaS.Domain.Exceptions;

namespace InvoiceSaaS.Domain.Validators;

public static class TenantDomainValidator
{
    public static void ValidateCompanyTenant(Company company, Guid tenantId)
    {
        if (company.TenantId != tenantId)
        {
            throw new DomainException($"Company {company.Id} does not belong to Tenant {tenantId}");
        }
    }

    public static void ValidateCustomerTenant(Customer customer, Company company)
    {
        if (customer.TenantId != company.TenantId)
        {
            throw new DomainException("Customer and Company must belong to the same Tenant.");
        }
    }

    public static void ValidateInvoiceTenant(Invoice invoice, Customer customer, Company company)
    {
        if (invoice.TenantId != customer.TenantId || invoice.TenantId != company.TenantId)
        {
            throw new DomainException("Invoice, Customer, and Company must all belong to the same Tenant.");
        }
    }
    
    public static void ValidateProductTenant(Product product, Company company)
    {
        if (product.TenantId != company.TenantId)
        {
            throw new DomainException("Product and Company must belong to the same Tenant.");
        }
    }
}
