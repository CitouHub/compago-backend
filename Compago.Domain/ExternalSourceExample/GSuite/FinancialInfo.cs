namespace Compago.Domain.ExternalSourceExample.GSuite
{
    public class FinancialInfo
    {
        public string Currency { get; set; } = null!;
        public List<InvoiceDescription> InvoiceDescritions = null!;
    }
}
