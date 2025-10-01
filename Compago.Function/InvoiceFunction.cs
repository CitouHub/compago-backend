using Compago.Common;
using Compago.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Compago.Function
{
    public class InvoiceFunction(
        ILogger<InvoiceFunction> logger,
        IDelegateService delegateService)
    {
        [Function("BillingGSuite2")]
        public async Task<IActionResult> RunBillingGSuite([HttpTrigger(AuthorizationLevel.Function, "get", Route = "billing/gsuite/{fromDate}/{toDate}")] HttpRequest _,
            DateTime fromDate,
            DateTime toDate,
            string? currency)
        {
            logger.LogDebug("{message}", @$"HTTP Function call to 
            {SupportedExternalSource.GSuite}, 
            {fromDate:yyyy-MM-dd}, 
            {toDate:yyyy-MM-dd}, 
            {currency}");

            var billing = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, fromDate, toDate, currency);
            return new OkObjectResult(billing);
        }

        [Function("BillingMicrosoftAzure2")]
        public async Task<IActionResult> RunBillingMicrosoftAzure([HttpTrigger(AuthorizationLevel.Function, "get", Route = "billing/microsoftazure/{fromDate}/{toDate}")] HttpRequest _,
            DateTime fromDate,
            DateTime toDate,
            string? currency)
        {
            logger.LogDebug("{message}", @$"HTTP Function call to 
            {SupportedExternalSource.MicrosoftAzure}, 
            {fromDate:yyyy-MM-dd}, 
            {toDate:yyyy-MM-dd}, 
            {currency}");

            var billing = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, currency);
            return new OkObjectResult(billing);
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "like/this/{fromDate}")] HttpRequest req, DateTime fromDate)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult($"Welcome to Azure Functions! {fromDate:yyyy-MM-dd}");
        }
    }
}