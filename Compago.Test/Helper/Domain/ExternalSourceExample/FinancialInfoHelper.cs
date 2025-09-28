using Compago.Domain.ExternalSourceExample.GSuite;

namespace Compago.Test.Helper.Domain.ExternalSourceExample
{
    public static class FinancialInfoHelper
    {
        public static FinancialInfo New(string? currency = null)
        {
            return new FinancialInfo()
            {
                Currency = currency ?? "SEK",
                InvoiceDescritions = []
            };
        }
    }
}
