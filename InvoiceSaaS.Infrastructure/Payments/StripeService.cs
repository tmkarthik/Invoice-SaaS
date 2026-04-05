using InvoiceSaaS.Application.Interfaces;
using InvoiceSaaS.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;

namespace InvoiceSaaS.Infrastructure.Payments;

public sealed class StripeService : IPaymentService
{
    private readonly string _secretKey;
    private readonly string _webhookSecret;
    private readonly IServiceProvider _serviceProvider;

    public StripeService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _secretKey = configuration["Stripe:SecretKey"] ?? throw new ArgumentNullException("Stripe SecretKey missing");
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? throw new ArgumentNullException("Stripe WebhookSecret missing");
        _serviceProvider = serviceProvider;
        StripeConfiguration.ApiKey = _secretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(Guid id, decimal amount, string currency, string successUrl, string cancelUrl, string customerEmail)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100), // Stripe uses cents
                        Currency = currency.ToLower(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Invoice Payment",
                        },
                    },
                    Quantity = 1,
                },
            ],
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            CustomerEmail = customerEmail,
            Metadata = new Dictionary<string, string>
            {
                { "InvoiceId", id.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<bool> ProcessWebhookAsync(string payload, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, _webhookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                var invoiceIdStr = session?.Metadata?.GetValueOrDefault("InvoiceId");
                
                if (Guid.TryParse(invoiceIdStr, out var invoiceId))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceSaaSDbContext>();
                    var invoice = await dbContext.Invoices.FindAsync(invoiceId);
                    
                    if (invoice != null)
                    {
                        invoice.SetStatus(InvoiceSaaS.Domain.Enums.InvoiceStatus.Paid);
                        await dbContext.SaveChangesAsync();
                        return true;
                    }
                }
            }

            return false;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}
