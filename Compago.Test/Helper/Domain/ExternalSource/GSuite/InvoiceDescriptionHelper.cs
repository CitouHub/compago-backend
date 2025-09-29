using Compago.Domain.ExternalSourceExample.GSuite;

namespace Compago.Test.Helper.Domain.ExternalSource.GSuite
{
    public static class InvoiceDescriptionHelper
    {
        public static InvoiceDescription New(
            string id = "1", 
            double cost = 12.34, 
            DateTime? invoiceDate = null)
        {
            return new InvoiceDescription()
            {
                Id = id,
                Cost = cost,
                InvoiceDate = invoiceDate ?? DateTime.UtcNow
            };
        }
    }
}
