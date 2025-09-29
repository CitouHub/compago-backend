using Compago.Data;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class InvoiceTagHelper
    {
        public static InvoiceTagDTO New(
            short tagId = 1,
            string invoiceId = "TestInvoiceId")
        {
            return new InvoiceTagDTO()
            {
                TagId = tagId,
                InvoiceId = invoiceId
            };
        }

        public static InvoiceTag NewDB(
            short tagId = 1,
            string invoiceId = "TestInvoiceId")
        {
            return new InvoiceTag()
            {
                TagId = tagId,
                InvoiceId = invoiceId
            };
        }
    }
}
