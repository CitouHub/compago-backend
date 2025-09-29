using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.MicosoftAzure;
using Compago.Service.Config;
using Compago.Service.CustomeException;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Compago.Service.ExternalSource.MicrosoftAzure
{
    public interface IMicrosoftAzureService
    {
        Task<BillingDTO?> GetBillingAsync(
            DateTime fromDate,
            DateTime toDate);
    }

    public class MicrosoftAzureService(
        ILogger<MicrosoftAzureService> logger,
        IMapper mapper,
        IOptions<ExternalSourceSettings.MicrosoftAzure> settings
    ) : IMicrosoftAzureService
    {
        public async Task<BillingDTO?> GetBillingAsync(
            DateTime fromDate,
            DateTime toDate)
        {
            logger.LogDebug("{message}", $"GET {settings.Value.URL}/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}");

            // #############################
            // ## HTTP Simulated API Call ##
            // #############################
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var task = new Task<string>(_ =>
            {
                try
                {
                    var response = ExampleResponse.MicosoftAzure.GetExample();
                    var requestedInvoices = response.Expenses.Monthly
                        .Where(_ => _.IssueDate >= fromDate && _.IssueDate <= toDate)
                        .ToList() ?? [];
                    response.Expenses.Monthly = requestedInvoices;

                    return JsonConvert.SerializeObject(response);
                } 
                catch (Exception ex)
                {
                    throw new ServiceException(ExceptionType.ExternalSourceCallError, ex);
                }
            }, null);
            task.Start();
            await task.WaitAsync(cancellationToken);

            var payload = JsonConvert.DeserializeObject<Payload>(task.Result);
            return payload != null ? mapper.Map<BillingDTO>(payload.Expenses) : null;
        }
    } 
}
