using Compago.API.ExceptionHandling;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
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
            [InlineData(Helper.HttpMethod.Delete, "invoicetag/1/1", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Compago.Common.Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class AddInvoiceTagAsync
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var request = new { name = (string?)null };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/invoicetag", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockInvoiceTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var invoiceTagDto = InvoiceTagHelper.New();
                var content = new StringContent(JsonConvert.SerializeObject(invoiceTagDto), Encoding.UTF8, "application/json");

                app.MockInvoiceTagService.AddInvoiceTagAsync(Arg.Any<InvoiceTagDTO>()).Returns(new InvoiceTagDTO());

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/invoicetag", content);
                var result = await response.Content.ReadFromJsonAsync<InvoiceTagDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).AddInvoiceTagAsync(Arg.Any<InvoiceTagDTO>());
            }
        }

        public class GetInvoiceTags
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/invoicetag/list/invalid");
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
                var tagId = (short)1;
                app.MockInvoiceTagService.GetInvoiceTagsAsync(tagId).Returns((List<InvoiceTagDTO>?)null);

                // Act
                var client = app.CreateClient();
                var response = await client.GetAsync($"{Constants.API_VERSION}/invoicetag/list/{tagId}");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).GetInvoiceTagsAsync(tagId);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var tagId = (short)1;
                app.MockInvoiceTagService.GetInvoiceTagsAsync(tagId).Returns([new(), new()]);

                // Act
                var client = app.CreateClient();
                var response = await client.GetAsync($"{Constants.API_VERSION}/invoicetag/list/{tagId}");
                var result = await response.Content.ReadFromJsonAsync<List<InvoiceTagDTO>>();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(2, result?.Count);
                await app.MockInvoiceTagService.Received(1).GetInvoiceTagsAsync(tagId);
            }
        }

        public class DeleteInvoiceTag
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/invoicetag/invoiceId/invalid");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockInvoiceTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                var invoiceId = "1";
                var tagId = (short)2;

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/invoicetag/{invoiceId}/{tagId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockInvoiceTagService.Received(1).DeleteInvoiceTagAsync(invoiceId, tagId);
            }
        }
    }
}
