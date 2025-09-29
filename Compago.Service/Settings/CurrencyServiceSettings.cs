using Compago.Service.CustomeException;

namespace Compago.Service.Settings
{
    public class CurrencyServiceSettings
    {
        public class EX
        {
            private string? APIKeyValue = null;
            public string APIKey
            {
                set { APIKeyValue = value; }
                get
                {
                    return APIKeyValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(CurrencyServiceSettings)}: {nameof(EX)}: {nameof(APIKey)} is missing/invalid");
                }
            }

            private string? URLValue = null;
            public string URL
            {
                set { URLValue = value; }
                get
                {
                    return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(CurrencyServiceSettings)}: {nameof(EX)}: {nameof(URL)} is missing/invalid");
                }
            }
        }
    }
}
