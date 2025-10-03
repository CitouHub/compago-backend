namespace Compago.Domain
{
    public class InvoiceDTO
    {
        public string Id { get; set; } = null!;
        public double Price { get; set; }
        public double? ExchangeRate { get; set; }
        public DateTime Date { get; set; }
        public List<InvoiceTagDTO> InvoiceTags { get; set; } = [];
    }
}
