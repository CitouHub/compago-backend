using Compago.Data;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class InvoiceTagHelper
    {
        public static InvoiceTagDTO New(
            string invoiceId = "TestInvoiceId",
            short tagId = 1)
        {
            return new InvoiceTagDTO()
            {
                InvoiceId = invoiceId,
                TagId = tagId
            };
        }

        public static InvoiceTag NewDb(
            string invoiceId = "TestInvoiceId",
            short tagId = 1)
        {
            return new InvoiceTag()
            {
                InvoiceId = invoiceId,
                TagId = tagId
            };
        }
    }
}
