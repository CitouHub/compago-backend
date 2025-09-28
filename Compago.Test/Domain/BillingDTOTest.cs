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
    }
}
