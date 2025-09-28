using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class BillingHelper
    {
        public static BillingDTO New(string? currency = null)
        {
            return new BillingDTO()
            {
                Currency = currency ?? "SEK"
            };
        }
    }
}
