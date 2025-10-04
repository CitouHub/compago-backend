using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;

namespace Compago.Test.Service.ExternalSource.MicrosoftAzure
{
    // Note:
    // Testing external sources access not needed as they are simulated
    public class MicrosoftAzureServiceTest : ServiceTest
    {
        public class GetInvoices
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var microsoftAzureService = new MicrosoftAzureService(_microsoftAzureServiceLogger, _mapper, _microsoftAzureDefaultOptions);

                // Act 
                var response = await microsoftAzureService.GetInvoicesAsync(new DateTime(2025, 01, 01), new DateTime(2025, 12, 31));

                // Assert
                Assert.NotNull(response);
                Assert.Equal(12, response.Count);
                Assert.True(response.All(_ => _.Currency != null));
            }
        }

        public class GetInvoice
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var microsoftAzureService = new MicrosoftAzureService(_microsoftAzureServiceLogger, _mapper, _microsoftAzureDefaultOptions);

                // Act 
                var response = await microsoftAzureService.GetInvoiceAsync(1);

                // Assert
                Assert.NotNull(response);
                Assert.NotEqual(null!, response.Currency);
            }
        }
    }
}
