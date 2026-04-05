using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSaaS.Infrastructure.Persistence.Configurations;

public sealed class TaxDefinitionConfiguration : IEntityTypeConfiguration<TaxDefinition>
{
    public void Configure(EntityTypeBuilder<TaxDefinition> builder)
    {
        builder.ToTable("TaxDefinitions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Rate).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.IsCompound).IsRequired();
        builder.Property(x => x.Priority).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne<InvoiceSaaS.Domain.Entities.Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
