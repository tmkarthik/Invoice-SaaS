using InvoiceSaaS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceSaaS.Infrastructure.Services;

public sealed class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        logger.LogInformation("--- MOCK EMAIL SENT ---");
        logger.LogInformation("To: {To}", to);
        logger.LogInformation("Subject: {Subject}", subject);
        logger.LogInformation("Body: {Body}", body);
        logger.LogInformation("-----------------------");
        
        return Task.CompletedTask;
    }
}
