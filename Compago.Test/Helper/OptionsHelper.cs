using Compago.Service.Config;
using Microsoft.Extensions.Options;

namespace Compago.Test.Helper
{
    public static class OptionsHelper
    {
        public static IOptions<ExternalSourceSettings.GSuite> DefineGSuiteSettingOptions(
            string username = "GSuiteUsername",
            string password = "GSuitePassword",
            string url = "https://test.com/api")
        {
            return Options.Create(new ExternalSourceSettings.GSuite()
            {
                Username = username,
                Password = password,
                URL = url
            });
        }

        public static IOptions<ExternalSourceSettings.MicrosoftAzure> DefineMicrosoftAzureSettingOptions(
            string accessId = "MicrosoftAzureAccessId",
            string subscription = "MicrosoftAzureSubscription",
            string invoiceAPIKey = "MicrosoftAzureInvoiceAPIKey",
            string url = "https://test.com/api")
        {
            return Options.Create(new ExternalSourceSettings.MicrosoftAzure()
            {
                AccessId = accessId,
                Subscription = subscription,
                InvoiceAPIKey = invoiceAPIKey,
                URL = url
            });
        }

        public static IOptions<CurrencyServiceSettings.EX> DefineEXSettingOptions(
            string apiKey = "EXAPIKey",
            string url = "https://test.com/api")
        {
            return Options.Create(new CurrencyServiceSettings.EX()
            {
                APIKey = apiKey,
                URL = url
            });
        }
    }
}
