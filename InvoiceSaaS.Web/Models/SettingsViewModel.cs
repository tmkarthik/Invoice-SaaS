using InvoiceSaaS.Application.DTOs;

namespace InvoiceSaaS.Web.Models;

public class SettingsViewModel
{
    public InvoiceSettingsDto InvoiceSettings { get; set; } = null!;
    public List<TaxDto> Taxes { get; set; } = [];
}
