using Compago.Service.ExternalSource.MicrosoftAzure;

namespace Compago.Test.Service.ExternalSource.MicrosoftAzure
{
    // Note:
    // Testing external sources access not needed as they are simulated
    public class MicrosoftAzureServiceTest : ServiceTest
    {
        public class GetBilling
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var microsoftAzureService = new MicrosoftAzureService(_microsoftAzureServiceLogger, _mapper, _microsoftAzureDefaultOptions);

                // Act 
                var response = await microsoftAzureService.GetBillingAsync(new DateTime(2025, 01, 01), new DateTime(2025, 12, 31));

                // Assert
                Assert.NotNull(response);
            }
        }
    }
}
