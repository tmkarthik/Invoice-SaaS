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
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<ITaxService, TaxService>();
        return services;
    }
}
