using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Authentication;
using InvoiceSaaS.Infrastructure.Configurations;
using InvoiceSaaS.Infrastructure.Persistence;
using InvoiceSaaS.Infrastructure.Repositories;
using InvoiceSaaS.Infrastructure.Payments;
using InvoiceSaaS.Infrastructure.Services;
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
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IPaymentService, StripeService>();
        services.AddScoped<IPdfService, PuppeteerPdfService>();
        services.AddSingleton<IEmailService, MockEmailService>();
        services.AddHostedService<PaymentReminderWorker>();
        services.AddDbContext<InvoiceSaaSDbContext>((serviceProvider, options) =>
        {
            var infraOptions = serviceProvider.GetRequiredService<IOptions<InfrastructureOptions>>().Value;
            var connectionString = string.IsNullOrWhiteSpace(infraOptions.ConnectionString)
                ? configuration.GetConnectionString("DefaultConnection")
                : infraOptions.ConnectionString;

            if (infraOptions.UseInMemoryRepository)
            {
                options.UseInMemoryDatabase("InvoiceSaaS")
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
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
            if (infraOptions.UseInMemoryRepository) return new InMemoryInvoiceRepository();
            return new EfInvoiceRepository(serviceProvider.GetRequiredService<InvoiceSaaSDbContext>());
        });

        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
