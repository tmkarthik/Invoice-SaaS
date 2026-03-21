using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
