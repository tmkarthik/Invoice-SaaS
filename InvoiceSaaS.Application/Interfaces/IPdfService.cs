namespace InvoiceSaaS.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GeneratePdfFromHtmlAsync(string html);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
