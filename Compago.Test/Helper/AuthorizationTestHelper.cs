using Compago.Common;
using Compago.Data;
using Compago.Domain;
using Compago.Test.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using Xunit.Abstractions;

namespace Compago.Test.Helper
{
    public class AuthorizationTestHelper(ITestOutputHelper output)
    {
        // Password = abc123
        private readonly string Username = "TestUser";
        private static readonly string CorrentPasswordHash = "zCjCUaEMdHUKVFTh+c6p7/hkwVgoRiBCU5ZeIfZrj88=";
        private static readonly string CorrectPasswordSalt = "GR3BmFPzSZtEwQZF268Atw=="; 

        private readonly List<Compago.Common.Role> AllRoles = [.. Enum.GetValues<Compago.Common.Role>().Cast<Compago.Common.Role>()];

        public async Task<int> TestAuthorize(HttpMethod httpMethod, string url, Compago.Common.Role[] authorizedRoles)
        {
            var app = new CompagoAPIMock();
            app.SetAuthorizationActive(true);
            var client = app.CreateClient();

            var unexpectedError = 0;
            foreach (var role in AllRoles)
            {
                HttpRequestMessage? request = null;
                HttpResponseMessage? response = null;
                var fullUrl = $"{Constants.API_VERSION}/{url}";

                app.MockUserService.GetUserSecurityCredentialsAsync($"{Username}{role}")
                    .Returns(new UserSecurityCredentialsDTO()
                    {
                        PasswordHash = CorrentPasswordHash,
                        PasswordHashSalt = CorrectPasswordSalt,
                        RoleId = role,
                    });

                switch (httpMethod)
                {
                    case HttpMethod.Get:
                        request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, fullUrl);
                        break;
                    case HttpMethod.Post:
                        request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, fullUrl);
                        break;
                    case HttpMethod.Put:
                        request = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, fullUrl);
                        break;
                    case HttpMethod.Delete:
                        request = new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, fullUrl);
                        break;
                }
                
                if (request != null)
                {
                    request.Headers.Add("username", $"{Username}{role}");
                    request.Headers.Add("password", "abc123");
                    response = await client.SendAsync(request);
                }

                if (authorizedRoles.Contains(role))
                {
                    if (response == null || HttpStatusCode.Unauthorized == response.StatusCode)
                    {
                        output.WriteLine($"Role {role} is expected to be Authorized but is Unauthorized for {fullUrl}, got {response?.StatusCode}");
                        unexpectedError++;
                    }
                }
                else
                {
                    if (response == null || HttpStatusCode.Unauthorized != response.StatusCode)
                    {
                        output.WriteLine($"Role {role} is expected to be Unauthorized but is Authorized for {fullUrl}, got {response?.StatusCode}");
                        unexpectedError++;
                    }
                }
            }

            return unexpectedError;
        }
    }
}
