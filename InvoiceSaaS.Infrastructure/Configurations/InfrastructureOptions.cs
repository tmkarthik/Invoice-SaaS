namespace InvoiceSaaS.Infrastructure.Configurations;

public sealed class InfrastructureOptions
{
    public const string SectionName = "Infrastructure";
    public bool UseInMemoryRepository { get; set; } = true;
    public string ConnectionString { get; set; } = string.Empty;
}
