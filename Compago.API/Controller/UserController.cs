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
    [AuthorizeRole(Role.Admin)]
    public class UserController(
        ILogger<UserController> logger,
        IUserService userService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<UserDTO>> AddUserAsync([FromBody] UserDTO userDto)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await userService.AddUserAsync(userDto));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> GetUserAsync(int userId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {userId}");

            return Ok(await userService.GetUserAsync(userId));
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<UserDTO>>> GetUsersAsync()
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await userService.GetUsersAsync());
        }

        [HttpPut("")]
        public async Task<ActionResult<UserDTO>> UpdateUserAsync([FromBody] UserDTO userDto)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");
                        
            return Ok(await userService.UpdateUserAsync(userDto));
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<UserDTO>> DeleteUserAsync(int userId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {userId}");

            await userService.DeleteUserAsync(userId);
            return Ok();
        }
    }
}
