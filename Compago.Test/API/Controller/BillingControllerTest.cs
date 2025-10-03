using Compago.API.ExceptionHandling;
using Compago.Common;
using Compago.Domain;
using Compago.Test.Helper;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Compago.Test.API.Controller
{
    public class BillingControllerTest
    {
        public class Authorization(ITestOutputHelper output)
        {
            private readonly AuthorizationTestHelper _authorizationTestHelper = new(output);

            [Theory]
            [InlineData(Helper.HttpMethod.Get, "billing/externalSource/2025-01-01/2025-01-01", Role.Admin, Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class GetBilling()
        {
            [Theory]
            [InlineData("invalid", "2025-01-01", "2025-01-01")]
            [InlineData("gsuite", "invalid", "2025-01-01")]
            [InlineData("gsuite", "2025-01-01", "invalid")]
            [InlineData("gsuite", "2025-13-01", "2025-01-01")]
            [InlineData("gsuite", "2025-01-01", "2025-13-01")]
            public async Task InvalidRequest(string supportedExternalSource, string fromDate, string toDate)
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                client.DefaultRequestHeaders.Add("X-Version", "1");

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/billing/
                    {supportedExternalSource}/{
                    fromDate}/
                    {toDate}");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                app.MockDelegateService.DidNotReceiveWithAnyArgs();
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                app.MockDelegateService.GetBillingAsync(
                    Arg.Any<SupportedExternalSource>(),
                    Arg.Any<DateTime>(),
                    Arg.Any<DateTime>(),
                    Arg.Any<string?>())
                    .Returns((BillingDTO?)null);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{SupportedExternalSource.GSuite}/{DateTime.UtcNow:yyyy-MM-dd}/{DateTime.UtcNow:yyyy-MM-dd}");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                await app.MockDelegateService
                    .Received(1)
                    .GetBillingAsync(Arg.Any<SupportedExternalSource>(),
                        Arg.Any<DateTime>(),
                        Arg.Any<DateTime>(),
                        Arg.Any<string?>());
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("SEK")]
            public async Task Success(string? currency)
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var supportedExternalSource = SupportedExternalSource.GSuite;
                var fromDate = new DateTime(2025, 01, 02);
                var toDate = new DateTime(2025, 03, 04);
                app.MockDelegateService.GetBillingAsync(
                    supportedExternalSource,
                    fromDate,
                    toDate,
                    currency)
                    .Returns(new BillingDTO());

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{supportedExternalSource}/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}?currency={currency}");
                var result = await response.Content.ReadFromJsonAsync<BillingDTO>();

                // Assert
                await app.MockDelegateService
                    .Received(1)
                    .GetBillingAsync(supportedExternalSource,
                        fromDate,
                        toDate,
                        currency);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(result);
            }
        }
    }
}
