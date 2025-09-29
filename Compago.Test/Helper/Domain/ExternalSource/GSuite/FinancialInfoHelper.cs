using Compago.Domain.ExternalSourceExample.GSuite;

namespace Compago.Test.Helper.Domain.ExternalSource.GSuite
{
    public static class FinancialInfoHelper
    {
        public static FinancialInfo New(string currency = "SEK")
        {
            return new FinancialInfo()
            {
                Currency = currency,
                InvoiceDescritions = []
            };
        }
    }
}
