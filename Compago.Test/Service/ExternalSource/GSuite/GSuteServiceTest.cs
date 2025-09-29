using Compago.Service.ExternalSource.GSuite;

namespace Compago.Test.Service.ExternalSource.GSuite
{
    // Note:
    // Testing external sources access not needed as they are simulated
    public class GSuteServiceTest : ServiceTest
    {
        public class GetBilling
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var gSuiteService = new GSuiteService(_gSuiteServiceLogger, _mapper, _gSuiteDefaultOptions);

                // Act 
                var response = await gSuiteService.GetBillingAsync(DateTime.UtcNow, DateTime.UtcNow);

                // Assert
                Assert.NotNull(response);
            }
        }
    }
}
