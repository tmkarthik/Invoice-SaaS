namespace InvoiceSaaS.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(Guid id, decimal amount, string currency, string successUrl, string cancelUrl, string customerEmail);
    Task<bool> ProcessWebhookAsync(string payload, string signature);
}
