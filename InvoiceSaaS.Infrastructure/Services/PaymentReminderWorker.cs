using InvoiceSaaS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvoiceSaaS.Infrastructure.Services;

public sealed class PaymentReminderWorker(
    IServiceProvider serviceProvider,
    ILogger<PaymentReminderWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Payment Reminder Worker running at: {Time}", DateTimeOffset.Now);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var invoices = await invoiceService.GetInvoicesAsync();
                var overdueInvoices = invoices.Where(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid");

                foreach (var invoice in overdueInvoices)
                {
                    logger.LogInformation("Sending reminder for Invoice {Number} to {Email}", invoice.InvoiceNumber, invoice.CustomerName);
                    
                    var subject = $"Payment Reminder: Invoice #{invoice.InvoiceNumber}";
                    var body = $@"Dear {invoice.CustomerName},
                    
                    This is a reminder that Invoice #{invoice.InvoiceNumber} was due on {invoice.DueDate.ToShortDateString()}.
                    The outstanding balance is {invoice.Amount.ToString("C")} {invoice.Currency}.
                    
                    Please pay at your earliest convenience.
                    
                    Thank you,
                    InvoiceCraft Support";

                    await emailService.SendEmailAsync(invoice.CustomerEmail ?? "", subject, body);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing payment reminders.");
            }

            // Run once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
