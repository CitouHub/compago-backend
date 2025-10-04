using Compago.Common;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class InvoiceHelper
    {
        public static InvoiceDTO New(
            string id = "Id",
            double price = 12.34,
            DateTime? date = null,
            double? exchangeRate = null,
            string currency = "SEK",
            string? originalCurrency = null,
            SupportedExternalSource supportedExternalSource = SupportedExternalSource.GSuite)
        {
            return new InvoiceDTO()
            {
                Id = id,
                Price = price,
                Date = date ?? DateTime.UtcNow,
                ExchangeRate = exchangeRate,
                Currency = currency,
                OriginalCurrency = originalCurrency,
                Source = supportedExternalSource
            };
        }
    }
}
