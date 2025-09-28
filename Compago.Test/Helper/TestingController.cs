using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Compago.Test.Helper
{
    [ApiController]
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class TestingController() : ControllerBase
    {
        [HttpGet("get")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}