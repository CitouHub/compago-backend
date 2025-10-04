using Compago.Test.Helper.Domain;

namespace Compago.Test.Domain
{
    public class InvoiceTest
    {
        [Fact]
        public static void CurrencyUpperCase()
        {
            // Arrange
            var currency = "smallCaps";

            // Act
            var invoice = InvoiceHelper.New(currency: currency);

            // Assert
            Assert.Equal(currency.ToUpper(), invoice.Currency);
        }

        [Fact]
        public static void CurrencyNull()
        {
            // Arrange
            var invoice = InvoiceHelper.New();
            invoice.Currency = null!;

            // Assert
            Assert.Null(invoice.Currency);
        }

        [Fact]
        public static void OriginalCurrencyUpperCase()
        {
            // Arrange
            var currency = "smallCaps";

            // Act
            var invoice = InvoiceHelper.New(originalCurrency: currency);

            // Assert
            Assert.Equal(currency.ToUpper(), invoice.OriginalCurrency);
        }

        [Fact]
        public static void OriginalCurrencyNull()
        {
            // Arrange
            var invoice = InvoiceHelper.New();
            invoice.OriginalCurrency = null!;

            // Assert
            Assert.Null(invoice.OriginalCurrency);
        }
    }
}
