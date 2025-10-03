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
        [HttpPost("{invoiceId}")]
        public async Task<ActionResult<List<InvoiceTagDTO>>> UpdateInvoiceTagAsync(string invoiceId, [FromBody] List<short> tagIds)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await invoiceTagService.UpdateInvoiceTagAsync(invoiceId, tagIds));
        }

        [HttpGet("list/{invoiceId}")]
        public async Task<ActionResult<List<InvoiceTagDTO>>> GetInvoiceTagsAsync(string invoiceId)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}");

            return Ok(await invoiceTagService.GetInvoiceTagsAsync(invoiceId));
        }
    }
}
