using Compago.API.ExceptionHandling;
using Compago.Domain;
using Compago.Test.Helper;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit.Abstractions;

namespace Compago.Test.API.Controller
{
    public class InvoiceTagControllerTest
    {
        public class Authorization(ITestOutputHelper output)
        {
            private readonly AuthorizationTestHelper _authorizationTestHelper = new(output);

            [Theory]
            [InlineData(Helper.HttpMethod.Post, "invoicetag", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            [InlineData(Helper.HttpMethod.Get, "invoicetag/list/1", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Compago.Common.Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class UpdateInvoiceTagAsync
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var request = new List<string>() { "invalid" };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/invoicetag/1", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockInvoiceTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var invoiceId = "1";
                app.MockInvoiceTagService.UpdateInvoiceTagAsync(invoiceId, Arg.Any<List<short>>()).Returns((List<InvoiceTagDTO>?)null);
                var content = new StringContent(JsonConvert.SerializeObject(new List<short>()), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/invoicetag/{invoiceId}", content);
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).UpdateInvoiceTagAsync(invoiceId, Arg.Any<List<short>>());
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                
                var invoiceId = "1";
                var request = new List<short>() { 1, 2 };
                app.MockInvoiceTagService.UpdateInvoiceTagAsync(invoiceId, Arg.Any<List<short>>()).Returns([new(), new()]);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/invoicetag/{invoiceId}", content);
                var result = await response.Content.ReadFromJsonAsync<List<InvoiceTagDTO>>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).UpdateInvoiceTagAsync(invoiceId, Arg.Any<List<short>>());
            }
        }

        public class GetInvoiceTags
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var invoiceId= "1";
                app.MockInvoiceTagService.GetInvoiceTagsAsync(invoiceId).Returns((List<InvoiceTagDTO>?)null);
                
                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/invoicetag/list/{invoiceId}");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).GetInvoiceTagsAsync(invoiceId);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var invoiceId = "1";
                app.MockInvoiceTagService.GetInvoiceTagsAsync(invoiceId).Returns([new(), new()]);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/invoicetag/list/{invoiceId}");
                var result = await response.Content.ReadFromJsonAsync<List<InvoiceTagDTO>>();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(2, result?.Count);
                await app.MockInvoiceTagService.Received(1).GetInvoiceTagsAsync(invoiceId);
            }
        }
    }
}
