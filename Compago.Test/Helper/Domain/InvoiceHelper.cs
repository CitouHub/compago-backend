using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class InvoiceHelper
    {
        public static InvoiceDTO New(
            string id = "Id",
            double price = 12.34,
            DateTime? date = null,
            double? exchangeRate = null)
        {
            return new InvoiceDTO()
            {
                Id = id,
                Price = price,
                Date = date ?? DateTime.Now,
                ExchangeRate = exchangeRate,
            };
        }
    }
}
