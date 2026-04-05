namespace InvoiceSaaS.Application.DTOs;

public record TaxDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public bool IsCompound { get; init; }
    public int Priority { get; init; }
}

public record CreateTaxDto
{
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public bool IsCompound { get; init; }
    public int Priority { get; init; }
}
