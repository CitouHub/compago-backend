using Compago.Service.CustomeException;

namespace Compago.Service.Config
{
    public class ExternalSourceSettings
    {
        public class GSuite
        {
            private string? UsernameValue = null;
            public string Username
            {
                set { UsernameValue = value; }
                get { return UsernameValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration, 
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(GSuite)}: {nameof(Username)} is missing/invalid"); 
                }
            }

            private string? PasswordValue = null;
            public string Password
            {
                set { PasswordValue = value; }
                get { return PasswordValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(GSuite)}: {nameof(Password)} is missing/invalid");
                }
            }

            private string? URLValue = null;
            public string URL
            {
                set { URLValue = value; }
                get { return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(GSuite)}: {nameof(URL)} is missing/invalid");
                }
            }
        }

        public class MicrosoftAzure
        {
            private string? AccessIdValue = null;
            public string AccessId
            {
                set { AccessIdValue = value; }
                get { return AccessIdValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(MicrosoftAzure)}: {nameof(AccessId)} is missing/invalid");
                }
            }

            private string? InvoiceAPIKeyValue = null;
            public string InvoiceAPIKey
            {
                set { InvoiceAPIKeyValue = value; }
                get { return InvoiceAPIKeyValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(MicrosoftAzure)}: {nameof(InvoiceAPIKey)} is missing/invalid");
                }
            }

            private string? SubscriptionValue = null;
            public string Subscription
            {
                set { SubscriptionValue = value; }
                get { return SubscriptionValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(MicrosoftAzure)}: {nameof(Subscription)} is missing/invalid");
                }
            }

            private string? URLValue = null;
            public string URL
            {
                set { URLValue = value; }
                get { return URLValue ?? throw new ServiceException(
                    ExceptionType.InvalidConfiguration,
                    details: $"{nameof(ExternalSourceSettings)}: {nameof(MicrosoftAzure)}: {nameof(URL)} is missing/invalid");
                }
            }
        }
    }
}
