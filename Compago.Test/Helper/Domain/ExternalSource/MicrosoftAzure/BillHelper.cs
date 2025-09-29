using Compago.Domain.ExternalSourceExample.MicosoftAzure;

namespace Compago.Test.Helper.Domain.ExternalSource.MicrosoftAzure
{
    public static class BillHelper
    {
        public static Bill New(
            long reference = 1,
            string moneyToPay = "12,34")
        {
            return new Bill()
            {
                Reference = reference,
                MoneyToPay = moneyToPay
            };
        }
    }
}
