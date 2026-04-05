using InvoiceSaaS.Application.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace InvoiceSaaS.Infrastructure.Services;

public sealed class PuppeteerPdfService : IPdfService
{
    public async Task<byte[]> GeneratePdfFromHtmlAsync(string html)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        
        await page.SetContentAsync(html);
        
        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20px",
                Bottom = "20px",
                Left = "20px",
                Right = "20px"
            }
        });
    }
}
