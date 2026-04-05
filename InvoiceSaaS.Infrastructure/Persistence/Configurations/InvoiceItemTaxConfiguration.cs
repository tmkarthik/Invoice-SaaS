using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSaaS.Infrastructure.Persistence.Configurations;

public sealed class InvoiceItemTaxConfiguration : IEntityTypeConfiguration<InvoiceItemTax>
{
    public void Configure(EntityTypeBuilder<InvoiceItemTax> builder)
    {
        builder.ToTable("InvoiceItemTaxes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TaxName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppliedRate).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();

        builder.HasOne(x => x.InvoiceItem)
            .WithMany(x => x.Taxes)
            .HasForeignKey(x => x.InvoiceItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TaxDefinition)
            .WithMany()
            .HasForeignKey(x => x.TaxDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
