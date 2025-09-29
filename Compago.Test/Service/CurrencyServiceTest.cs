using Compago.Service;

namespace Compago.Test.Service
{
    // Note:
    // Testing fetching currency exchange rate not needed as it is simulated
    public class CurrencyServiceTest : ServiceTest
    {
        public class GetExchangeRate
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var currencyService = new CurrencyService();

                // Act 
                var response = await currencyService.GetExchangeRateAsync("FROM", "TO", DateTime.UtcNow);

                // Assert
                Assert.NotEqual(0, response);
            }

            [Fact]
            public async Task Success_SameFromTo()
            {
                // Arrange
                var currencyService = new CurrencyService();

                // Act 
                var response = await currencyService.GetExchangeRateAsync("SaME", "saME", DateTime.UtcNow);

                // Assert
                Assert.Equal(1, response);
            }
        }
    }
}
