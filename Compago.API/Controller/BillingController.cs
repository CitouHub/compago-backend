using Asp.Versioning;
using Compago.Common;
using Compago.Domain;
using Compago.Service;
using Microsoft.AspNetCore.Mvc;

namespace Compago.API.Controller
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BillingController(
        ILogger<BillingController> logger,
        IDelegateService delegateService) : ControllerBase
    {
        [HttpGet("{supportedExternalSource}/{fromDate}/{toDate}")]
        public async Task<ActionResult<BillingDTO?>> GetBillingAsync(
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
            return await delegateService.GetBillingAsync(supportedExternalSource, fromDate, toDate, currency);
        }
    }
}
