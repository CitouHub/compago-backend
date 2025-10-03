namespace Compago.Domain
{
    public class BillingDTO
    {
        private string? OrigialCurrencyValue;

        public string? OrigialCurrency
        {
            set { OrigialCurrencyValue = value; }
            get
            {
                return OrigialCurrencyValue?.ToUpper() ?? null!;
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

        public Common.SupportedExternalSource Source { get; set; }

        public List<InvoiceDTO> Invoices { get; set; } = [];
    }
}
