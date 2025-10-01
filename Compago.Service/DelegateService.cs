using Compago.Common;
using Compago.Domain;
using Compago.Service.CustomeException;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Service.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        IOptions<CurrencyServiceSettings.EX> settings) : IDelegateService
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
                {currency},
                {settings.Value.APIKey}");

            var billing = supportedExternalSource switch
            {
                SupportedExternalSource.GSuite => await gSuiteService.GetBillingAsync(fromDate, toDate),
                SupportedExternalSource.MicrosoftAzure => await microsoftAzureService.GetBillingAsync(fromDate, toDate),
                _ => throw new ServiceException(ExceptionType.ExternalSourceNotSupported)
            };

            if (billing != null && currency != null && currency.Replace(" ", "").Length > 0)
            {
                foreach (var invoice in billing.Invoices)
                {
                    var exchangeRate = await currencyService.GetExchangeRateAsync(billing.Currency, currency, invoice.Date);
                    invoice.Price = Math.Round(invoice.Price * exchangeRate, 2);
                    invoice.ExchangeRate = exchangeRate;
                }

                billing.OrigialCurrency = billing.Currency;
                billing.Currency = currency;
            }

            return billing;
        }
    }
}
