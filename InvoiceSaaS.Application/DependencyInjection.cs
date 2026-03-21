using System.Reflection;
using FluentValidation;
using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        return services;
    }
}
