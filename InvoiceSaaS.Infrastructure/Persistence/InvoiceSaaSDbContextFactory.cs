using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InvoiceSaaS.Infrastructure.Persistence;

public class InvoiceSaaSDbContextFactory : IDesignTimeDbContextFactory<InvoiceSaaSDbContext>
{
    public InvoiceSaaSDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../InvoiceSaaS.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<InvoiceSaaSDbContext>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration.GetSection("Infrastructure:ConnectionString").Value;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Could not find connection string in appsettings.json of API project.");
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new InvoiceSaaSDbContext(optionsBuilder.Options, null!); // ITenantProvider is null at design time
    }
}
