using Compago.Domain.ExternalSourceExample.MicosoftAzure;

namespace Compago.Test.Helper.Domain.ExternalSource.MicrosoftAzure
{
    public static class ExpensesHelper
    {
        public static Expenses New(string currency = "SEK")
        {
            return new Expenses()
            {
                Currency = currency,
                Monthly = []
            };
        }
    }
}
