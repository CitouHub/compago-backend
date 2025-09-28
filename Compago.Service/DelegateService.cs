using Compago.Common;
using Compago.Domain;
using Compago.Service.CustomeException;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;

namespace Compago.Service
{
    public interface IDelegateService
    {
        Task<BillingDTO> GetBillingAsync(
            SupportedExternalSource supportedExternalSource, 
            DateTime fromDate, 
            DateTime toDate,
            string? currency = null);
    }

    public class DelegateService(
        IGSuiteService gSuiteService,
        IMicrosoftAzureService microsoftAzureService,
        ICurrencyService currencyService) : IDelegateService
    {
        public async Task<BillingDTO> GetBillingAsync(
            SupportedExternalSource supportedExternalSource,
            DateTime fromDate,
            DateTime toDate,
            string? currency = null)
        {
            var billing = supportedExternalSource switch
            {
                SupportedExternalSource.GSuite => await gSuiteService.GetBillingAsync(fromDate, toDate),
                SupportedExternalSource.MicrosoftAzure => await microsoftAzureService.GetBillingAsync(fromDate, toDate),
                _ => throw new ServiceException(ExceptionType.ExternalSourceNotSupported)
            };

            if (currency != null)
            {
                billing.Invoices.ForEach(async _ => await UpdateInvoiceAsync(_, billing.Currency, currency));
                billing.Currency = currency.ToUpper();
            }

            return billing;
        }

        private async Task UpdateInvoiceAsync(InvoiceDTO invoice, string currentCurrency, string requestedCurrency)
        {
            var (amount, exchangeRate) = await currencyService.ConvertAsync(invoice.Price, currentCurrency, requestedCurrency, invoice.Date);
            invoice.Price = amount;
            invoice.ExchangeRate = exchangeRate;
        }
    }
}
