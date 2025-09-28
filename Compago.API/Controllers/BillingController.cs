using Asp.Versioning;
using Compago.Common;
using Compago.Domain;
using Compago.Service;
using Microsoft.AspNetCore.Mvc;

namespace Compago.API.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BillingController(
        ILogger<BillingController> logger,
        IDelegateService delegateService) : ControllerBase
    {
        [HttpGet("{supporedExternalSource}/{fromDate}/{toDate}")]
        public async Task<BillingDTO> GetBillingAsync(
            SupportedExternalSource supporedExternalSource, 
            DateTime fromDate,
            DateTime toDate,
            string? currency = null)
        {
            logger.LogDebug("{message}", @$"Call to 
                {nameof(ControllerContext.ActionDescriptor.ActionName)} 
                {supporedExternalSource}
                {fromDate:yyyy-MM-dd} 
                {toDate:yyyy-MM-dd}
                {currency}");
            return await delegateService.GetBillingAsync(supporedExternalSource, fromDate, toDate, currency);
        }
    }
}
