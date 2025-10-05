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
    public class InvoiceControllerTest
    {
        public class Authorization(ITestOutputHelper output)
        {
            private readonly AuthorizationTestHelper _authorizationTestHelper = new(output);

            [Theory]
            [InlineData(Helper.HttpMethod.Get, "invoice/externalSource/2025-01-01/2025-01-01", Role.Admin, Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class GetInvoices()
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
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();
                client.DefaultRequestHeaders.Add("X-Version", "1");

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/invoice/
                    {supportedExternalSource}/
                    {fromDate}/
                    {toDate}");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                app.MockExternalSourceService.DidNotReceiveWithAnyArgs();
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();
                app.MockExternalSourceService.GetInvoicesAsync(
                    Arg.Any<SupportedExternalSource>(),
                    Arg.Any<DateTime>(),
                    Arg.Any<DateTime>(),
                    Arg.Any<string?>())
                    .Returns((List<InvoiceDTO>?)null);

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/invoice/
                    {SupportedExternalSource.GSuite}/
                    {DateTime.UtcNow:yyyy-MM-dd}/
                    {DateTime.UtcNow:yyyy-MM-dd}");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                await app.MockExternalSourceService
                    .Received(1)
                    .GetInvoicesAsync(Arg.Any<SupportedExternalSource>(),
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
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();

                var supportedExternalSource = SupportedExternalSource.GSuite;
                var fromDate = new DateTime(2025, 01, 02);
                var toDate = new DateTime(2025, 03, 04);
                app.MockExternalSourceService.GetInvoicesAsync(
                    supportedExternalSource,
                    fromDate,
                    toDate,
                    currency)
                    .Returns([new InvoiceDTO()]);

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/invoice/
                    {supportedExternalSource}/
                    {fromDate:yyyy-MM-dd}/
                    {toDate:yyyy-MM-dd}?currency={currency}");
                var result = await response.Content.ReadFromJsonAsync<List<InvoiceDTO>>();

                // Assert
                await app.MockExternalSourceService
                    .Received(1)
                    .GetInvoicesAsync(supportedExternalSource,
                        fromDate,
                        toDate,
                        currency);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(result);
            }
        }

        public class GetInvoice
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();
                
                client.DefaultRequestHeaders.Add("X-Version", "1");

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/invoice/invalid/1");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                app.MockExternalSourceService.DidNotReceiveWithAnyArgs();
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();

                app.MockExternalSourceService.GetInvoiceAsync(
                    Arg.Any<SupportedExternalSource>(), 
                    Arg.Any<string>()).Returns(new InvoiceDTO());

                // Act
                var response = await client.GetAsync(@$"{Constants.API_VERSION}/invoice/{SupportedExternalSource.GSuite}/1");
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockExternalSourceService.Received(1).GetInvoiceAsync(
                    Arg.Any<SupportedExternalSource>(),
                    Arg.Any<string>());
            }
        }
    }
}
