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
    public class InvoiceTagController(
        ILogger<InvoiceTagController> logger,
        IInvoiceTagService invoiceTagService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<InvoiceTagDTO>> AddInvoiceTagAsync([FromBody] InvoiceTagDTO invoiceTagDto)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await invoiceTagService.AddInvoiceTagAsync(invoiceTagDto));
        }

        [HttpGet("list/{tagId}")]
        public async Task<ActionResult<List<InvoiceTagDTO>>> GetInvoiceTagsAsync(int tagId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await invoiceTagService.GetInvoiceTagsAsync(tagId));
        }

        [HttpDelete("{invoiceId}/{tagId}")]
        public async Task<ActionResult<InvoiceTagDTO>> DeleteInvoiceTagAsync(string invoiceId, short tagId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            await invoiceTagService.DeleteInvoiceTagAsync(invoiceId, tagId);
            return Ok();
        }
    }
}
