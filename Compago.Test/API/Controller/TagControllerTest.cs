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
    public class TagControllerTest
    {
        public class Authorization(ITestOutputHelper output)
        {
            private readonly AuthorizationTestHelper _authorizationTestHelper = new(output);

            [Theory]
            [InlineData(Helper.HttpMethod.Post, "tag", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            [InlineData(Helper.HttpMethod.Get, "tag/list", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            [InlineData(Helper.HttpMethod.Get, "tag/1", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            [InlineData(Helper.HttpMethod.Put, "tag", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            [InlineData(Helper.HttpMethod.Delete, "tag/1", Compago.Common.Role.Admin, Compago.Common.Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Compago.Common.Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class AddTagAsync
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var request = new { name = (string?)null };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/tag", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var tagDto = TagHelper.New();
                var content = new StringContent(JsonConvert.SerializeObject(tagDto), Encoding.UTF8, "application/json");

                app.MockTagService.AddTagAsync(Arg.Any<TagDTO>()).Returns(new TagDTO());

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/tag", content);
                var result = await response.Content.ReadFromJsonAsync<TagDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockTagService.Received(1).AddTagAsync(Arg.Any<TagDTO>());
            }
        }

        public class GetTag
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/tag/invalid");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var tagId = (short)1;
                var tagDto = TagHelper.New(id: tagId);

                app.MockTagService.GetTagAsync(tagId).Returns(tagDto);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/tag/{tagId}");
                var result = await response.Content.ReadFromJsonAsync<TagDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockTagService.Received(1).GetTagAsync(tagId);
            }
        }

        public class GetTags
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                app.MockTagService.GetTagsAsync().Returns((List<TagDTO>?)null);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/tag/list");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                await app.MockTagService.Received(1).GetTagsAsync();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                app.MockTagService.GetTagsAsync().Returns([new(), new()]);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/tag/list");
                var result = await response.Content.ReadFromJsonAsync<List<TagDTO>>();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(2, result?.Count);
                await app.MockTagService.Received(1).GetTagsAsync();
            }
        }

        public class UpdateTag
        {
            [Theory]
            [InlineData(null, "tagname")]
            [InlineData((short)1, null)]
            public async Task InvalidRequest(short? roleId, string? tagname)
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                var request = new { roleId, tagname };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PutAsync($"{Constants.API_VERSION}/tag", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                app.MockTagService.UpdateTagAsync(Arg.Any<TagDTO>()).Returns(new TagDTO());

                var tagDto = TagHelper.New();
                var content = new StringContent(JsonConvert.SerializeObject(tagDto), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PutAsync($"{Constants.API_VERSION}/tag", content);
                var result = await response.Content.ReadFromJsonAsync<TagDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockTagService.Received(1).UpdateTagAsync(Arg.Any<TagDTO>());
            }
        }

        public class DeleteTag
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/tag/invalid");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockTagService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                app.SetAuthorizationActive(false);
                var tagId = 1;

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/tag/{tagId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockTagService.Received(1).DeleteTagAsync(tagId);
            }
        }
    }
}
