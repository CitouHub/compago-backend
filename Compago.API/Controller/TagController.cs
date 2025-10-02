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
    public class TagController(
        ILogger<TagController> logger,
        ITagService tagService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<TagDTO>> AddTagAsync([FromBody] TagDTO tagDto)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await tagService.AddTagAsync(tagDto));
        }

        [HttpGet("{tagId}")]
        public async Task<ActionResult<TagDTO>> GetTagAsync(int tagId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {tagId}");

            return Ok(await tagService.GetTagAsync(tagId));
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<TagDTO>>> GetTagsAsync()
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await tagService.GetTagsAsync());
        }

        [HttpPut("")]
        public async Task<ActionResult<TagDTO>> UpdateTagAsync([FromBody] TagDTO tagDto)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");
                        
            return Ok(await tagService.UpdateTagAsync(tagDto));
        }

        [HttpDelete("{tagId}")]
        public async Task<ActionResult<TagDTO>> DeleteTagAsync(int tagId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {tagId}");

            await tagService.DeleteTagAsync(tagId);
            return Ok();
        }
    }
}
