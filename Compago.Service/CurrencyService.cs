using Compago.Service.CustomeException;
using Compago.Service.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Compago.Service
{
    public interface ICurrencyService
    {
        Task<double> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime date);
    }

    public class CurrencyService(
        ILogger<CurrencyService> logger,
        IOptions<CurrencyServiceSettings.EX> settings) : ICurrencyService
    {
        public async Task<double> GetExchangeRateAsync(
            string fromCurrency, 
            string toCurrency, 
            DateTime date)
        {
            logger.LogDebug("{message}", $"AUTH {settings.Value.APIKey}");
            logger.LogDebug("{message}", $"GET {settings.Value.URL}/{fromCurrency}/{toCurrency}/{date:yyyy-MM-dd}");

            fromCurrency = fromCurrency.ToUpper();
            toCurrency = toCurrency.ToUpper();

            if (fromCurrency != toCurrency)
            {
                //// #############################
                //// ## HTTP Simulated API Call ##
                //// #############################
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                var task = new Task<double>(_ =>
                {
                    try
                    {
                        var longBytes = BitConverter.GetBytes((double)0).ToList();

                        var baseFactorKey = $"{fromCurrency}{toCurrency}";
                        var baseFactorKeyBytes = Encoding.ASCII.GetBytes(baseFactorKey).ToList().Concat(longBytes);
                        var baseFactorKeyLong = BitConverter.ToInt64([.. baseFactorKeyBytes], 0);
                        var baseFactorBase = baseFactorKeyLong % 10;
                        var baseFactorInverse = baseFactorKeyLong % 2 == 0;

                        var dateFactorModifierKey = $"{date:yyyy-MM-dd}";
                        var dateFactorModifierKeyBytes = Encoding.ASCII.GetBytes(dateFactorModifierKey).ToList().Concat(longBytes);
                        var dateFactorModifierKeyLong = BitConverter.ToInt64([.. dateFactorModifierKeyBytes], 0);
                        var dateFactorModifierKeyString = ((decimal)baseFactorKeyLong / dateFactorModifierKeyLong).ToString();
                        var dateFactorModifier = short.Parse(dateFactorModifierKeyString.Substring(dateFactorModifierKeyString.Length - 2, 2));

                        var exchangeRate = Math.Round((baseFactorInverse ? 1.0 / baseFactorBase : baseFactorBase) * (0.95 + 0.001 * dateFactorModifier), 2);

                        return exchangeRate;
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException(ExceptionType.CurrencyServiceCallError, ex, $"{fromCurrency} => {toCurrency}");
                    }
                }, null);
                task.Start();
                await task.WaitAsync(cancellationToken);

                return task.Result;
            }

            return 1;
        }
    }
}
