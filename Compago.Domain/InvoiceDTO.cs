using Compago.Common;

namespace Compago.Domain
{
    public class InvoiceDTO
    {
        public string Id { get; set; } = null!;
        public double Price { get; set; }
        public double? ExchangeRate { get; set; }
        public DateTime Date { get; set; }
        public SupportedExternalSource Source { get; set; }
        public List<InvoiceTagDTO> InvoiceTags { get; set; } = [];

        private string? OriginalCurrencyValue;

        public string? OriginalCurrency
        {
            set { OriginalCurrencyValue = value; }
            get
            {
                return OriginalCurrencyValue?.ToUpper() ?? null!;
            }
        }

        private string CurrencyValue = null!;
        public string Currency
        {
            set { CurrencyValue = value; }
            get
            {
                return CurrencyValue?.ToUpper() ?? null!;
            }
        }
    }
}
