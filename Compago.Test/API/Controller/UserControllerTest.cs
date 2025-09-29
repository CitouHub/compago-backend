using Compago.API.ExceptionHandling;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Compago.Test.API.Controller
{
    public class UserControllerTest
    {
        public class AddUserAsync
        {
            [Theory]
            [InlineData(null, "username")]
            [InlineData((short)1, null)]
            public async Task InvalidRequest(short? roleId, string? username)
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var request = new { roleId, username };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/user", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockUserService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var userDto = UserHelper.New();
                var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");

                app.MockUserService.AddUserAsync(Arg.Any<UserDTO>()).Returns(new UserDTO());

                // Act
                var response = await client.PostAsync($"{Constants.API_VERSION}/user", content);
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockUserService.Received(1).AddUserAsync(Arg.Any<UserDTO>());
            }
        }

        public class GetUser
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/user/invalid");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockUserService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var userId = 1;
                var userDto = UserHelper.New(id: userId);

                app.MockUserService.GetUserAsync(userId).Returns(userDto);

                // Act
                var response = await client.GetAsync($"{Constants.API_VERSION}/user/{userId}");
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockUserService.Received(1).GetUserAsync(userId);
            }
        }

        public class GetUsers
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.MockUserService.GetUsersAsync().Returns((List<UserDTO>?)null);

                // Act
                var client = app.CreateClient();
                var response = await client.GetAsync($"{Constants.API_VERSION}/user/list");
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.True(string.IsNullOrEmpty(result));
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                await app.MockUserService.Received(1).GetUsersAsync();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.MockUserService.GetUsersAsync().Returns([new(), new()]);

                // Act
                var client = app.CreateClient();
                var response = await client.GetAsync($"{Constants.API_VERSION}/user/list");
                var result = await response.Content.ReadFromJsonAsync<List<UserDTO>>();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(2, result?.Count);
                await app.MockUserService.Received(1).GetUsersAsync();
            }
        }

        public class UpdateUser
        {
            [Theory]
            [InlineData(null, "username")]
            [InlineData((short)1, null)]
            public async Task InvalidRequest(short? roleId, string? username)
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var request = new { roleId, username };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PutAsync($"{Constants.API_VERSION}/user", content);
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockUserService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                var userDto = UserHelper.New();
                var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");

                app.MockUserService.UpdateUserAsync(Arg.Any<UserDTO>()).Returns(new UserDTO());

                // Act
                var response = await client.PutAsync($"{Constants.API_VERSION}/user", content);
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockUserService.Received(1).UpdateUserAsync(Arg.Any<UserDTO>());
            }
        }

        public class DeleteUser
        {
            [Fact]
            public async Task InvalidRequest()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/user/invalid");
                var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

                // Assert
                Assert.Equal((int)HttpStatusCode.BadRequest, result?.Status);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                app.MockUserService.DidNotReceiveWithAnyArgs();
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                var client = app.CreateClient();
                var userId = 1;

                // Act
                var response = await client.DeleteAsync($"{Constants.API_VERSION}/user/{userId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                await app.MockUserService.Received(1).DeleteUserAsync(userId);
            }
        }
    }
}
