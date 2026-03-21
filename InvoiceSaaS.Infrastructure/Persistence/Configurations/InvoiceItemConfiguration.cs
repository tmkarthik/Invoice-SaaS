using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSaaS.Infrastructure.Persistence.Configurations;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TaxRate)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.HasIndex(x => x.InvoiceId);
        builder.HasIndex(x => x.ProductId);
    }
}
