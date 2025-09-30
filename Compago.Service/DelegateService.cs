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
        ICurrencyService currencyService) : IDelegateService
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

            if (billing != null && currency != null && currency.Replace(" ", "").Length > 0)
            {
                billing.Invoices.ForEach(async _ => await UpdateInvoiceAsync(_, billing.Currency, currency));
                billing.Currency = currency;
            }

            return billing;
        }

        private async Task UpdateInvoiceAsync(InvoiceDTO invoice, string currentCurrency, string requestedCurrency)
        {
            var exchangeRate = await currencyService.GetExchangeRateAsync(currentCurrency, requestedCurrency, invoice.Date);
            invoice.Price = Math.Round(invoice.Price * exchangeRate, 2);
            invoice.ExchangeRate = exchangeRate;
        }
    }
}
