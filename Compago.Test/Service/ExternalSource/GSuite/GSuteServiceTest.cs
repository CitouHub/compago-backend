using Compago.Service.ExternalSource.GSuite;

namespace Compago.Test.Service.ExternalSource.GSuite
{
    // Note:
    // Testing external sources access not needed as they are simulated
    public class GSuteServiceTest : ServiceTest
    {
        public class GetInvoices
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var gSuiteService = new GSuiteService(_gSuiteServiceLogger, _mapper, _gSuiteDefaultOptions);

                // Act 
                var response = await gSuiteService.GetInvoicesAsync(new DateTime(2025, 01, 01), new DateTime(2025, 12, 31));

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
                var gSuiteService = new GSuiteService(_gSuiteServiceLogger, _mapper, _gSuiteDefaultOptions);

                // Act 
                var response = await gSuiteService.GetInvoiceAsync("04d856fd-1ec6-4038-8f9e-3a94bffc4fc6");

                // Assert
                Assert.NotNull(response);
                Assert.NotEqual(null!, response.Currency);
            }
        }
    }
}
