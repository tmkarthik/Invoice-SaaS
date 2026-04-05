namespace InvoiceSaaS.Web.Models;

public class LineItemViewModel
{
    public string Description { get; set; } = "";
    public int Qty { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public List<Guid> SelectedTaxIds { get; set; } = new();
    public decimal Amount => Qty * UnitPrice;
}

public class CompanyInfoViewModel
{
    public string Name { get; set; } = "Acme Design Studio";
    public string Email { get; set; } = "hello@acmestudio.com";
    public string Phone { get; set; } = "+1 (415) 555-0123";
    public string Website { get; set; } = "www.acmestudio.com";
    public string Address { get; set; } = "123 Creative Ave, Suite 400, San Francisco, CA 94103";
}

public class CustomerInfoViewModel
{
    public string Name { get; set; } = "TechFlow Inc.";
    public string ContactPerson { get; set; } = "Mark Johnson";
    public string Email { get; set; } = "mark@techflow.io";
    public string Phone { get; set; } = "+1 (212) 555-0199";
    public string BillingAddress { get; set; } = "456 Startup Blvd, New York, NY 10001";
}

public class InvoiceDetailsViewModel
{
    public string InvoiceNumber { get; set; } = "INV-2024-0042";
    public string Currency { get; set; } = "USD ($)";
    public string IssueDate { get; set; } = "2024-04-01";
    public string DueDate { get; set; } = "2024-04-30";
}

public class TemplateEditorViewModel
{
    public string TemplateName { get; set; } = "Professional Invoice v2";
    public string SelectedLayerName { get; set; } = "Logo";
}

public class TaxBreakdownViewModel
{
    public string Name { get; set; } = "";
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

public class TemplateDataViewModel
{
    public CompanyInfoViewModel Company { get; set; } = new();
    public CustomerInfoViewModel Customer { get; set; } = new();
    public InvoiceDetailsViewModel InvoiceDetails { get; set; } = new();
    public List<LineItemViewModel> LineItems { get; set; } = new()
    {
        new() { Description = "Brand Identity Design", Qty = 1, UnitPrice = 2500 },
        new() { Description = "Website Design", Qty = 1, UnitPrice = 4800 },
        new() { Description = "UI Component Library", Qty = 1, UnitPrice = 1800 },
    };
    public List<InvoiceSaaS.Application.DTOs.TaxDto> AvailableTaxes { get; set; } = new();
    public List<TaxBreakdownViewModel> TaxBreakdown { get; set; } = new();
    
    public decimal Subtotal => LineItems.Sum(i => i.Amount);
    public decimal Tax => TaxBreakdown.Sum(x => x.Amount);
    public decimal Discount => 0m;
    public decimal Total => Subtotal + Tax - Discount;
}

public class TemplatePreviewViewModel : TemplateDataViewModel { }

public class TemplateSummaryViewModel
{
    public string Name { get; set; } = "";
    public string ColorVariant { get; set; } = "";
    public string MetaLabel { get; set; } = "";
    public bool IsDefault { get; set; }
}

public class TemplateListViewModel
{
    public List<TemplateSummaryViewModel> Templates { get; set; } = new();
}

public class VersionEntryViewModel
{
    public int VersionNumber { get; set; }
    public string Date { get; set; } = "";
    public string ChangeNotes { get; set; } = "";
    public bool IsCurrent { get; set; }
}

public class VersionHistoryViewModel
{
    public string TemplateName { get; set; } = "Professional Invoice v2";
    public List<VersionEntryViewModel> Versions { get; set; } = new();
}
