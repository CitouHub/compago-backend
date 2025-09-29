using Compago.Service.Settings;
using Compago.Service.CustomeException;

namespace Compago.Test.Service.Settings
{
    public class ExternalSourceSettingsTest
    {
        public class GSuite
        {
            [Theory]
            [InlineData(null, "password", "url", nameof(ExternalSourceSettings.GSuite.Username))]
            [InlineData("username", null, "url", nameof(ExternalSourceSettings.GSuite.Password))]
            [InlineData("username", "password", null, nameof(ExternalSourceSettings.GSuite.URL))]
            public void InvalidMissing(string? username, string? password, string? url, string expectedMissingInvalid)
            {
                // Arrange
                var settings = new ExternalSourceSettings.GSuite()
                {
                    Username = username ?? null!,
                    Password = password ?? null!,
                    URL = url ?? null!
                };

                // Act
                ServiceException? exception = null;
                try
                {
                    _ = settings.Username;
                    _ = settings.Password;
                    _ = settings.URL;
                }
                catch (ServiceException ex)
                {
                    exception = ex;
                }

                // Assert
                Assert.NotNull(exception);
                Assert.Contains(expectedMissingInvalid, exception.Message);
                Assert.Contains(nameof(ExternalSourceSettings), exception.Message);
                Assert.Contains(nameof(ExternalSourceSettings.GSuite), exception.Message);
            }

            [Fact]
            public void Valid()
            {
                // Arrange
                var settings = new ExternalSourceSettings.GSuite()
                {
                    Username = "Username",
                    Password = "Password",
                    URL = "URL"
                };

                // Act
                ServiceException? exception = null;
                try
                {
                    _ = settings.Username;
                    _ = settings.Password;
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

        public class MicrosoftAzure
        {
            [Theory]
            [InlineData(null, "invoiceAPIKey", "subscription", "url", nameof(ExternalSourceSettings.MicrosoftAzure.AccessId))]
            [InlineData("accessId", null, "subscription", "url", nameof(ExternalSourceSettings.MicrosoftAzure.InvoiceAPIKey))]
            [InlineData("accessId", "invoiceAPIKey", null, "url", nameof(ExternalSourceSettings.MicrosoftAzure.Subscription))]
            [InlineData("accessId", "invoiceAPIKey", "subscription", null, nameof(ExternalSourceSettings.MicrosoftAzure.URL))]
            public void InvalidMissing(string? accessId, string? invoiceAPIKey, string? subscription, string? url, string expectedMissingInvalid)
            {
                // Arrange
                var settings = new ExternalSourceSettings.MicrosoftAzure()
                {
                    AccessId = accessId ?? null!,
                    InvoiceAPIKey = invoiceAPIKey ?? null!,
                    Subscription = subscription ?? null!,
                    URL = url ?? null!
                };

                // Act
                ServiceException? exception = null;
                try
                {
                    _ = settings.AccessId;
                    _ = settings.InvoiceAPIKey;
                    _ = settings.Subscription;
                    _ = settings.URL;
                }
                catch (ServiceException ex)
                {
                    exception = ex;
                }

                // Assert
                Assert.NotNull(exception);
                Assert.Contains(expectedMissingInvalid, exception.Message);
                Assert.Contains(nameof(ExternalSourceSettings), exception.Message);
                Assert.Contains(nameof(ExternalSourceSettings.MicrosoftAzure), exception.Message);
            }

            [Fact]
            public void Valid()
            {
                // Arrange
                var settings = new ExternalSourceSettings.MicrosoftAzure()
                {
                    AccessId = "AccessId",
                    InvoiceAPIKey = "InvoiceAPIKey",
                    Subscription = "Subscription",
                    URL = "URL"
                };

                // Act
                ServiceException? exception = null;
                try
                {
                    _ = settings.AccessId;
                    _ = settings.InvoiceAPIKey;
                    _ = settings.Subscription;
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
}
