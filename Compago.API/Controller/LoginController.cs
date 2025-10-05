using Asp.Versioning;
using Compago.API.Security;
using Compago.Common;
using Compago.Domain;
using Compago.Service;
using Microsoft.AspNetCore.Mvc;

namespace Compago.API.Controller
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [AuthorizeRole(Role.Admin, Role.User)]
    public class LoginController(
        ILogger<LoginController> logger,
        IUserService userService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<Role>> LoginSync()
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            var username = Request.Headers["username"];
            var userSecurityCredentials = await userService.GetUserSecurityCredentialsAsync(username!);

            return Ok(new UserDTO()
            {
                RoleId = userSecurityCredentials.RoleId
            });
        }
    }
}
