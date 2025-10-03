using Compago.Common;
using Compago.Domain;
using Compago.Service.CustomeException;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Microsoft.Extensions.Logging;

namespace Compago.Service
{
    public interface IDelegateService
    {
        Task<BillingDTO?> GetBillingAsync(
            SupportedExternalSource supportedExternalSource, 
            DateTime fromDate, 
            DateTime toDate,
            string? currency = null);
    }

    public class DelegateService(
        ILogger<DelegateService> logger,
        IGSuiteService gSuiteService,
        IMicrosoftAzureService microsoftAzureService,
        ICurrencyService currencyService,
        IInvoiceTagService invoiceTagService) : IDelegateService
    {
        public async Task<BillingDTO?> GetBillingAsync(
            SupportedExternalSource supportedExternalSource,
            DateTime fromDate,
            DateTime toDate,
            string? currency = null)
        {
            logger.LogDebug("{message}", @$"{supportedExternalSource}, 
                {fromDate:yyyy-MM-dd}, 
                {toDate:yyyy-MM-dd}, 
                {currency}");

            var billing = supportedExternalSource switch
            {
                SupportedExternalSource.GSuite => await gSuiteService.GetBillingAsync(fromDate, toDate),
                SupportedExternalSource.MicrosoftAzure => await microsoftAzureService.GetBillingAsync(fromDate, toDate),
                _ => throw new ServiceException(ExceptionType.ExternalSourceNotSupported)
            };

            if (billing != null)
            {
                foreach (var invoice in billing.Invoices)
                {
                    if (currency != null && currency.Replace(" ", "").Length > 0)
                    {
                        var exchangeRate = await currencyService.GetExchangeRateAsync(billing.Currency, currency, invoice.Date);
                        invoice.Price = Math.Round(invoice.Price * exchangeRate, 2);
                        invoice.ExchangeRate = exchangeRate;
                    }         

                    var invoiceTags = await invoiceTagService.GetInvoiceTagsAsync(invoice.Id);
                    invoice.InvoiceTags = invoiceTags ?? [];
                }

                billing.OrigialCurrency = currency != null ? billing.Currency : null;
                billing.Currency = currency ?? billing.Currency;
                billing.Source = supportedExternalSource;
            }

            return billing;
        }
    }
}
