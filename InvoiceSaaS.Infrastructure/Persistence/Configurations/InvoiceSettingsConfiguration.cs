using InvoiceSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceSaaS.Infrastructure.Persistence.Configurations;

public sealed class InvoiceSettingsConfiguration : IEntityTypeConfiguration<InvoiceSettings>
{
    public void Configure(EntityTypeBuilder<InvoiceSettings> builder)
    {
        builder.ToTable("InvoiceSettings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Prefix).HasMaxLength(10).IsRequired();
        builder.Property(x => x.CurrentNumber).IsRequired();

        builder.HasIndex(x => x.TenantId).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne<InvoiceSaaS.Domain.Entities.Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
