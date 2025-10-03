using Compago.Common;
using Compago.Domain;
using Compago.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Compago.API.Security
{
    [AttributeUsage(AttributeTargets.All)]
    public class AuthorizeUserFilter(
        IUserService userService,
        ICacheService cacheService,
        IOptions<GeneralSettings> settings,
        Role[] roles) : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (settings.Value.AuthorizationActive == true)
            {
                var username = (string?)context.HttpContext.Request.Headers["username"];
                var password = (string?)context.HttpContext.Request.Headers["password"];

                if (username != null && password != null)
                {
                    try
                    {
                        var userSecurityCredentials = userService.GetUserSecurityCredentialsAsync(username).Result;
                        if (userSecurityCredentials != null)
                        {
                            if (roles == null || roles.Contains(userSecurityCredentials.RoleId) == true)
                            {
                                var valid = PasswordHandler.ValidatePassword(
                                    password, 
                                    userSecurityCredentials.PasswordHash,
                                    userSecurityCredentials.PasswordHashSalt);

                                cacheService.Set<UserSecurityCredentialsDTO>(userSecurityCredentials);

                                if (valid == false)
                                {
                                    throw new UnauthorizedAccessException();
                                }
                            }
                            else
                            {
                                throw new UnauthorizedAccessException();
                            }
                        }
                        else
                        {
                            throw new UnauthorizedAccessException();
                        }
                    } 
                    catch
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
