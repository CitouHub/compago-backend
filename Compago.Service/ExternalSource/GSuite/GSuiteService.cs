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
        Task<List<InvoiceDTO>?> GetInvoicesAsync(DateTime fromDate, DateTime toDate);
        Task<InvoiceDTO?> GetInvoiceAsync(string id);
    }

    public class GSuiteService(
        ILogger<GSuiteService> logger,
        IMapper mapper,
        IOptions<ExternalSourceSettings.GSuite> settings
    ) : IGSuiteService
    {
        public async Task<InvoiceDTO?> GetInvoiceAsync(string id)
        {
            logger.LogDebug("{message}", $"AUTH {settings.Value.Username} {settings.Value.Password}");
            logger.LogDebug("{message}", $"GET {settings.Value.URL}/{id}");

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
                            .Where(_ => _.Id == id)
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

            var data = JsonConvert.DeserializeObject<Info>(task.Result);
            var invoices = data != null ? mapper.Map<List<InvoiceDTO>>(data.FinancialInfo.InvoiceDescritions) : null;
            (invoices ?? []).ForEach(_ => _.Currency = data?.FinancialInfo.Currency ?? null!);

            return invoices?.FirstOrDefault();
        }

        public async Task<List<InvoiceDTO>?> GetInvoicesAsync(
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

            var data = JsonConvert.DeserializeObject<Info>(task.Result);
            var invoices = data != null ? mapper.Map<List<InvoiceDTO>>(data.FinancialInfo.InvoiceDescritions) : null;
            (invoices ?? []).ForEach(_ => _.Currency = data?.FinancialInfo.Currency ?? null!);

            return invoices;
        }
    }
}
