namespace Compago.Domain
{
    public class BillingDTO
    {
        public string Currency { get; set; } = null!;
        public List<InvoiceDTO> Invoices { get; set; } = [];
    }
}
