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
        IExternalSourceService externalSourceService)
    {
        [Function("InvoicesGSuite")]
        public async Task<IActionResult> RunInvoicesGSuite([HttpTrigger(AuthorizationLevel.Function, "get", Route = "invoices/gsuite/{fromDate}/{toDate}")] HttpRequest _,
            DateTime fromDate,
            DateTime toDate,
            string? currency)
        {
            logger.LogDebug("{message}", @$"HTTP Function call to 
            {SupportedExternalSource.GSuite}, 
            {fromDate:yyyy-MM-dd}, 
            {toDate:yyyy-MM-dd}, 
            {currency}");

            var invoices = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, fromDate, toDate, currency);
            return new OkObjectResult(invoices);
        }

        [Function("InvoicesMicrosoftAzure")]
        public async Task<IActionResult> RunInvoicesMicrosoftAzure([HttpTrigger(AuthorizationLevel.Function, "get", Route = "invoices/microsoftazure/{fromDate}/{toDate}")] HttpRequest _,
            DateTime fromDate,
            DateTime toDate,
            string? currency)
        {
            logger.LogDebug("{message}", @$"HTTP Function call to 
            {SupportedExternalSource.MicrosoftAzure}, 
            {fromDate:yyyy-MM-dd}, 
            {toDate:yyyy-MM-dd}, 
            {currency}");

            var invoices = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, currency);
            return new OkObjectResult(invoices);
        }
    }
}