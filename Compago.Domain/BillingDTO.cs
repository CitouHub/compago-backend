namespace Compago.Domain
{
    public class BillingDTO
    {
        private string CurrencyValue = null!;
        public string Currency
        {
            set { CurrencyValue = value; }
            get
            {
                return CurrencyValue?.ToUpper() ?? null!;
            }
        }

        public List<InvoiceDTO> Invoices { get; set; } = [];
    }
}
