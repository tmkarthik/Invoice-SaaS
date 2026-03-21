namespace InvoiceSaaS.Application.Validators;

public static class InvoiceQueryValidator
{
    public static bool IsValidTenant(Guid tenantId) => tenantId != Guid.Empty;
}
