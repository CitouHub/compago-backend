using Compago.Service.CustomeException;
using System.Text;

namespace Compago.Service
{
    public interface ICurrencyService
    {
        Task<(double amount, double? exchangeRate)> ConvertAsync(double amount, string from, string to, DateTime date);
    }

    public class CurrencyService() : ICurrencyService
    {
        public async Task<(double amount, double? exchangeRate)> ConvertAsync(
            double amount, 
            string fromCurrency, 
            string toCurrency, 
            DateTime date)
        {
            fromCurrency = fromCurrency.ToUpper();
            toCurrency = toCurrency.ToUpper();

            if (fromCurrency != toCurrency)
            {
                // #############################
                // ## HTTP Simulated API Call ##
                // #############################
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                var convertTask = new Task<(double amount, double? exchangeRate)>(_ =>
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

                        var exchangeRate = Math.Round((baseFactorInverse ? 1 / baseFactorBase : baseFactorBase) * (0.95 + 0.001 * dateFactorModifier), 2);
                        var toCurrencyAmount = Math.Round(amount * exchangeRate, 2);

                        return (toCurrencyAmount, exchangeRate);
                    }
                    catch (Exception ex)
                    {
                        throw new ServiceException(ExceptionType.CurrencyServiceCallError, ex, $"{fromCurrency} => {toCurrency}");
                    }
                }, null);
                convertTask.Start();
                await convertTask.WaitAsync(cancellationToken);

                return convertTask.Result;
            }

            return (amount, null);
        }
    }
}
