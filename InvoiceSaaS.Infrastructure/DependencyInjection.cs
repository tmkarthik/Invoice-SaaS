using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Configurations;
using InvoiceSaaS.Infrastructure.Persistence;
using InvoiceSaaS.Infrastructure.Repositories;
using InvoiceSaaS.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InvoiceSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InfrastructureOptions>(configuration.GetSection(InfrastructureOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddDbContext<InvoiceSaaSDbContext>((serviceProvider, options) =>
        {
            var infraOptions = serviceProvider.GetRequiredService<IOptions<InfrastructureOptions>>().Value;
            var connectionString = string.IsNullOrWhiteSpace(infraOptions.ConnectionString)
                ? configuration.GetConnectionString("DefaultConnection")
                : infraOptions.ConnectionString;

            if (infraOptions.UseInMemoryRepository)
            {
                options.UseInMemoryDatabase("InvoiceSaaS");
                return;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("A SQL Server connection string must be configured.");
            }

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IInvoiceRepository>(serviceProvider =>
        {
            var infraOptions = serviceProvider.GetRequiredService<IOptions<InfrastructureOptions>>().Value;
            return infraOptions.UseInMemoryRepository
                ? new InMemoryInvoiceRepository()
                : new EfInvoiceRepository(serviceProvider.GetRequiredService<InvoiceSaaSDbContext>());
        });

        return services;
    }
}
