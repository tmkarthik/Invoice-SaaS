using System.Reflection;
using InvoiceSaaS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSaaS.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static void ApplyTenantFilter(this ModelBuilder modelBuilder, InvoiceSaaSDbContext context)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType))
            .Select(t => t.ClrType);

        foreach (var type in entityTypes)
        {
            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(ApplyFilter), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(type);

            method?.Invoke(null, [modelBuilder, context]);
        }
    }

    private static void ApplyFilter<T>(ModelBuilder modelBuilder, InvoiceSaaSDbContext context) where T : BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => (context.IsAdmin || e.TenantId == context.CurrentTenantId) && !e.IsDeleted);
    }
}
