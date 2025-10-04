using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.GSuite;
using Compago.Domain.ExternalSourceExample.MicosoftAzure;
using Compago.Service.CustomeException;
using Compago.Service.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Compago.Service.ExternalSource.MicrosoftAzure
{
    public interface IMicrosoftAzureService
    {
        Task<List<InvoiceDTO>?> GetInvoicesAsync(DateTime fromDate, DateTime toDate);
        Task<InvoiceDTO?> GetInvoiceAsync(long reference);
    }

    public class MicrosoftAzureService(
        ILogger<MicrosoftAzureService> logger,
        IMapper mapper,
        IOptions<ExternalSourceSettings.MicrosoftAzure> settings
    ) : IMicrosoftAzureService
    {
        public async Task<List<InvoiceDTO>?> GetInvoicesAsync(DateTime fromDate, DateTime toDate)
        {
            logger.LogDebug("{message}", $"AUTH {settings.Value.AccessId} {settings.Value.InvoiceAPIKey} {settings.Value.Subscription}");
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

            var data = JsonConvert.DeserializeObject<Payload>(task.Result);
            var invoices = data != null ? mapper.Map<List<InvoiceDTO>>(data.Expenses.Monthly) : null;
            (invoices ?? []).ForEach(_ => _.Currency = data?.Expenses.Currency ?? null!);

            return invoices;
        }

        public async Task<InvoiceDTO?> GetInvoiceAsync(long reference)
        {
            logger.LogDebug("{message}", $"AUTH {settings.Value.AccessId} {settings.Value.InvoiceAPIKey} {settings.Value.Subscription}");
            logger.LogDebug("{message}", $"GET {settings.Value.URL}/{reference}");

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
                        .Where(_ => _.Bill.Reference == reference)
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

            var data = JsonConvert.DeserializeObject<Payload>(task.Result);
            var invoices = data != null ? mapper.Map<List<InvoiceDTO>>(data.Expenses.Monthly) : null;
            (invoices ?? []).ForEach(_ => _.Currency = data?.Expenses.Currency ?? null!);

            return invoices?.FirstOrDefault();
        }
    } 
}
