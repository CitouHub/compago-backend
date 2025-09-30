using Compago.Test.Helper.Domain;

namespace Compago.Test.Domain
{
    public class BillingDTOTest
    {
        [Fact]
        public static void CurrencyUpperCase()
        {
            // Arrange
            var currency = "smallCaps";

            // Act
            var billing = BillingHelper.New(currency: currency);

            // Assert
            Assert.Equal(currency.ToUpper(), billing.Currency);
        }

        [Fact]
        public static void CurrencyNull()
        {
            // Arrange
            var billing = BillingHelper.New();
            billing.Currency = null!;

            // Assert
            Assert.Null(billing.Currency);
        }

        [Fact]
        public static void OriginalCurrencyUpperCase()
        {
            // Arrange
            var currency = "smallCaps";

            // Act
            var billing = BillingHelper.New(originalCurrency: currency);

            // Assert
            Assert.Equal(currency.ToUpper(), billing.OrigialCurrency);
        }

        [Fact]
        public static void OriginalCurrencyNull()
        {
            // Arrange
            var billing = BillingHelper.New();
            billing.OrigialCurrency = null!;

            // Assert
            Assert.Null(billing.OrigialCurrency);
        }
    }
}
