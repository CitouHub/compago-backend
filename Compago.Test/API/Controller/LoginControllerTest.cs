using Compago.Common;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using NSubstitute;
using System.Net;
using Xunit.Abstractions;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Compago.Test.API.Controller
{
    public class LoginControllerTest
    {
        public class Authorization(ITestOutputHelper output)
        {
            private readonly AuthorizationTestHelper _authorizationTestHelper = new(output);

            [Theory]
            [InlineData(Helper.HttpMethod.Post, "login", Role.Admin, Role.User)]
            public async Task AuthorizeRoles(Helper.HttpMethod httpMethod, string url, params Role[] authorizedRole)
            {
                // Act
                var unexpectedError = await _authorizationTestHelper.TestAuthorize(httpMethod, url, authorizedRole);

                // Assert
                Assert.Equal(0, unexpectedError);
            }
        }

        public class Login
        {
            [Fact]
            public async Task Success()
            {
                // Arrange
                var app = new CompagoAPIMock();
                app.SetAuthorizationActive(false);
                var client = app.CreateClient();

                var username = "TestUser";
                var userSecurityCredentials = UserSecurityCredentialsHelper.New();
                app.MockUserService.GetUserSecurityCredentialsAsync(username).Returns(userSecurityCredentials);

                // Act
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"http://localhost/{Constants.API_VERSION}/login"),
                    Content = null
                };
                request.Headers.Add("username", username);
                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                _ = Enum.TryParse(result, out Role roleId);
                Assert.Equal(userSecurityCredentials.RoleId, roleId);

                await app.MockUserService.Received(1).GetUserSecurityCredentialsAsync(username);
            }
        }
    }
}
