using Azure;
using Compago.Common;
using Compago.Service.CustomeException;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace Compago.Test.API.Security
{
    public class AuthorizeUserFilterTest
    {
        private readonly string testUrl = $"{Constants.API_VERSION}/user/list";
        private readonly Role correctRole = Role.Admin;
        private readonly Role incorrectRole = Role.User;

        [Theory]
        [InlineData(null, "abc123")]
        [InlineData("TestUser", null)]
        public async Task Unauthorized_Missing_UsernameOrPassword(string? username, string? password)
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.SetAuthorizationActive(true);
            var client = app.CreateClient();

            HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, testUrl);
            request.Headers.Add("username", username);
            request.Headers.Add("password", password);

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Unauthorized_NoUser()
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.SetAuthorizationActive(true);
            var client = app.CreateClient();

            var username = "TestUser";
            HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, testUrl);
            request.Headers.Add("username", username);
            request.Headers.Add("password", "TestPassword");

            app.MockUserService.GetUserSecurityCredentialsAsync(username)
                .Throws(new ServiceException(ExceptionType.ItemNotFound));

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Unauthorized_WrongRole()
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.SetAuthorizationActive(true);
            var client = app.CreateClient();

            var username = "TestUser";
            HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, testUrl);
            request.Headers.Add("username", username);
            request.Headers.Add("password", "TestPassword");

            app.MockUserService.GetUserSecurityCredentialsAsync(username)
                .Returns(UserSecurityCredentialsHelper.New(roleId: incorrectRole));

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Unauthorized_WrongPassword()
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.SetAuthorizationActive(true);
            var client = app.CreateClient();

            var username = "TestUser";
            HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, testUrl);
            request.Headers.Add("username", username);
            request.Headers.Add("password", "TestPassword");

            app.MockUserService.GetUserSecurityCredentialsAsync(username)
                .Returns(UserSecurityCredentialsHelper.New(roleId: correctRole));

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
