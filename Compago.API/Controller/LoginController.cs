using Asp.Versioning;
using Compago.API.Security;
using Compago.Common;
using Microsoft.AspNetCore.Mvc;

namespace Compago.API.Controller
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [AuthorizeRole(Role.Admin, Role.User)]
    public class LoginController(ILogger<LoginController> logger) : ControllerBase
    {
        [HttpPost("")]
        public ActionResult<bool> LoginSync()
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(true);
        }
    }
}
