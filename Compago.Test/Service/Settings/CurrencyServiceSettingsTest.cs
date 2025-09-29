using Compago.Service.Settings;
using Compago.Service.CustomeException;

namespace Compago.Test.Service.Settings
{
    public class CurrencyServiceSettingsTest
    {
        [Theory]
        [InlineData(null, "url", nameof(CurrencyServiceSettings.EX.APIKey))]
        [InlineData("apiKey", null, nameof(CurrencyServiceSettings.EX.URL))]
        public void InvalidMissing(string? apiKey, string? url, string expectedMissingInvalid)
        {
            // Arrange
            var settings = new CurrencyServiceSettings.EX()
            {
                APIKey = apiKey ?? null!,
                URL = url ?? null!
            };

            // Act
            ServiceException? exception = null;
            try
            {
                _ = settings.APIKey;
                _ = settings.URL;
            }
            catch (ServiceException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(expectedMissingInvalid, exception.Message);
            Assert.Contains(nameof(CurrencyServiceSettings), exception.Message);
            Assert.Contains(nameof(CurrencyServiceSettings.EX), exception.Message);
        }

        [Fact]
        public void Valid()
        {
            // Arrange
            var settings = new CurrencyServiceSettings.EX()
            {
                APIKey = "APIKey",
                URL = "URL"
            };

            // Act
            ServiceException? exception = null;
            try
            {
                _ = settings.APIKey;
                _ = settings.URL;
            }
            catch (ServiceException ex)
            {
                exception = ex;
            }

            // Assert
            Assert.Null(exception);
        }
    }
}
