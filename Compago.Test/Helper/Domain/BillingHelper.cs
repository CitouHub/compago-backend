using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class BillingHelper
    {
        public static BillingDTO New(string currency = "SEK", string originalCurrency = "SEK")
        {
            return new BillingDTO()
            {
                Currency = currency,
                OrigialCurrency = originalCurrency,
            };
        }
    }
}
