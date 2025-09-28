using Compago.Service.CustomeException;

namespace Compago.Service.Config
{
    public class CurrencyServiceSettings
    {
        public class EX
        {
            private readonly string? APIKeyValue;
            public string APIKey
            {
                get
                {
                    return APIKeyValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details:  $"EX: {nameof(APIKey)} is missing/invalid");
                }
            }

            private readonly string? URLValue;
            public string URL
            {
                get
                {
                    return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"EX: {nameof(URL)} is missing/invalid");
                }
            }
        }
    }
}
