using Compago.Domain.ExternalSourceExample.GSuite;

namespace Compago.Test.Helper.Domain.ExternalSourceExample
{
    public static class InvoiceDescriptionHelper
    {
        public static InvoiceDescription New(
            string? id = null, 
            double? cost = null, 
            DateTime? invoiceDate = null)
        {
            return new InvoiceDescription()
            {
                Id = id ?? "1",
                Cost = cost ?? 0,
                InvoiceDate = invoiceDate ?? DateTime.UtcNow
            };
        }
    }
}
