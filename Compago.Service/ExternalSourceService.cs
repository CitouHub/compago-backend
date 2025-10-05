using Compago.Common;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Microsoft.Extensions.Logging;

namespace Compago.Service
{
    public interface IExternalSourceService
    {
        Task<List<InvoiceDTO>?> GetInvoicesAsync(
            SupportedExternalSource supportedExternalSource, 
            DateTime fromDate, 
            DateTime toDate,
            string? currency = null);

        Task<InvoiceDTO?> GetInvoiceAsync(
            SupportedExternalSource supportedExternalSource,
            string invoiceId,
            string? currency = null);
    }

    public class ExternalSourceService(
        ILogger<ExternalSourceService> logger,
        IGSuiteService gSuiteService,
        IMicrosoftAzureService microsoftAzureService,
        ICurrencyService currencyService,
        IInvoiceTagService invoiceTagService) : IExternalSourceService
    {
        public async Task<InvoiceDTO?> GetInvoiceAsync(
            SupportedExternalSource supportedExternalSource, 
            string invoiceId,
            string? currency = null)
        {
            logger.LogDebug("{message}", @$"{supportedExternalSource}, 
                {invoiceId},  
                {currency}");

            var invoice = supportedExternalSource switch
            {
                SupportedExternalSource.GSuite => await gSuiteService.GetInvoiceAsync(invoiceId),
                SupportedExternalSource.MicrosoftAzure => await microsoftAzureService.GetInvoiceAsync(long.Parse(invoiceId)),
                _ => throw new ServiceException(ExceptionType.ExternalSourceNotSupported)
            };

            if (invoice != null)
            {
                await MapInvoiceAsync(currencyService, invoiceTagService, invoice, supportedExternalSource, currency);
            } 
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{supportedExternalSource} data for {invoiceId} not found");
            }

            return invoice;
        }

        public async Task<List<InvoiceDTO>?> GetInvoicesAsync(
            SupportedExternalSource supportedExternalSource,
            DateTime fromDate,
            DateTime toDate,
            string? currency = null)
        {
            logger.LogDebug("{message}", @$"{supportedExternalSource}, 
                {fromDate:yyyy-MM-dd}, 
                {toDate:yyyy-MM-dd}, 
                {currency}");

            var invoices = supportedExternalSource switch
            {
                SupportedExternalSource.GSuite => await gSuiteService.GetInvoicesAsync(fromDate, toDate),
                SupportedExternalSource.MicrosoftAzure => await microsoftAzureService.GetInvoicesAsync(fromDate, toDate),
                _ => throw new ServiceException(ExceptionType.ExternalSourceNotSupported)
            };

            if (invoices != null)
            {
                foreach (var invoice in invoices)
                {
                    await MapInvoiceAsync(currencyService, invoiceTagService, invoice, supportedExternalSource, currency);
                }
            }

            return invoices;
        }

        private static async Task MapInvoiceAsync(
            ICurrencyService currencyService,
            IInvoiceTagService invoiceTagService,
            InvoiceDTO invoice,
            SupportedExternalSource supportedExternalSource,
            string? currency)
        {
            if (currency != null && currency.Replace(" ", "").Length > 0)
            {
                var exchangeRate = await currencyService.GetExchangeRateAsync(supportedExternalSource, invoice.Currency, currency, invoice.Date);
                invoice.Price = Math.Round(invoice.Price * exchangeRate, 2);
                invoice.ExchangeRate = exchangeRate;
            }

            var invoiceTags = await invoiceTagService.GetInvoiceTagsAsync(invoice.Id);
            invoice.InvoiceTags = invoiceTags ?? [];

            invoice.OriginalCurrency = currency != null ? invoice.Currency : null;
            invoice.Currency = currency ?? invoice.Currency;
            invoice.Source = supportedExternalSource;
        }
    }
}
