using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.GSuite;
using Compago.Service.Settings;
using Compago.Service.CustomeException;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Compago.Service.ExternalSource.GSuite
{
    public interface IGSuiteService
    {
        Task<BillingDTO?> GetBillingAsync(
            DateTime fromDate,
            DateTime toDate);
    }

    public class GSuiteService(
        ILogger<GSuiteService> logger,
        IMapper mapper,
        IOptions<ExternalSourceSettings.GSuite> settings
    ) : IGSuiteService
    {
        public async Task<BillingDTO?> GetBillingAsync(
            DateTime fromDate,
            DateTime toDate)
        {
            logger.LogDebug("{message}", $"AUTH {settings.Value.Username} {settings.Value.Password}");
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
                    var response = ExampleResponse.GetExample();
                    if (response.FinancialInfo != null)
                    {
                        var requestedInvoices = response.FinancialInfo.InvoiceDescritions?
                                .Where(_ => _.InvoiceDate >= fromDate && _.InvoiceDate <= toDate)
                                .ToList() ?? [];
                        response.FinancialInfo.InvoiceDescritions = requestedInvoices;
                    }

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    throw new ServiceException(ExceptionType.ExternalSourceCallError, ex);
                }
            }, null);
            task.Start();
            await task.WaitAsync(cancellationToken);

            var data = JsonConvert.DeserializeObject<Data>(task.Result);
            return data != null ? mapper.Map<BillingDTO>(data.FinancialInfo) : null;
        }
    }
}
