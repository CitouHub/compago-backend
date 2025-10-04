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
    public class InvoiceController(
        ILogger<InvoiceController> logger,
        IExternalSourceService externalSourceService) : ControllerBase
    {
        [AuthorizeRole(Role.Admin, Role.User)]
        [HttpGet("{supportedExternalSource}/{fromDate}/{toDate}")]
        public async Task<ActionResult<List<InvoiceDTO>?>> GetInvoicesAsync(
            SupportedExternalSource supportedExternalSource, 
            DateTime fromDate,
            DateTime toDate,
            [FromQuery] string? currency = null)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {supportedExternalSource}, 
                {fromDate:yyyy-MM-dd}, 
                {toDate:yyyy-MM-dd}, 
                {currency}");
            return Ok(await externalSourceService.GetInvoicesAsync(supportedExternalSource, fromDate, toDate, currency));
        }

        [AuthorizeRole(Role.Admin, Role.User)]
        [HttpGet("{supportedExternalSource}/{invoiceId}")]
        public async Task<ActionResult<InvoiceDTO?>> GetInvoiceAsync(
            SupportedExternalSource supportedExternalSource,
            string invoiceId,
            [FromQuery] string? currency = null)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)}, 
                {supportedExternalSource}, 
                {invoiceId}, 
                {currency}");
            return Ok(await externalSourceService.GetInvoiceAsync(supportedExternalSource, invoiceId, currency));
        }
    }
}
