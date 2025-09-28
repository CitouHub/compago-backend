using Compago.Service.CustomeException;

namespace Compago.Service.Config
{
    public class ExternalSourceSettings
    {
        public class GSuite
        {
            private readonly string? UsernameValue;
            public string Username
            {
                get { return UsernameValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "GSuite: Username is missing/invalid"); }
            }

            private readonly string? PasswordValue;
            public string Password
            {
                get { return PasswordValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "GSuite: Password is missing/invalid"); }
            }

            private readonly string? URLValue;
            public string URL
            {
                get { return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "GSuite: URL is missing/invalid"); }
            }
        }

        public class MicrosoftAzure
        {
            private readonly string? AccessIdVAlue;
            public string AccessId
            {
                get { return AccessIdVAlue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "MicrosoftAzure: AccessId is missing/invalid"); }
            }

            private readonly string? InvoiceAPIKeyValue;
            public string InvoiceAPIKey
            {
                get { return InvoiceAPIKey ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "MicrosoftAzure: InvoiceAPIKey is missing/invalid"); }
            }

            private readonly string? SubscriptionValue;
            public string Subscription
            {
                get { return Subscription ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "MicrosoftAzure: Subscription is missing/invalid"); }
            }

            private readonly string? URLValue;
            public string URL
            {
                get { return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: "MicrosoftAzure: URL is missing/invalid"); }
            }
        }
    }
}
