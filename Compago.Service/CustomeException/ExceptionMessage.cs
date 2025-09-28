namespace Compago.Service.CustomeException
{
    public static class ExceptionMessage
    {
        public static readonly string InvoiceNotFound = "Requested invoice could not be found.";
        public static readonly string ExternalSourceNotSupported = "The given external source is not supported.";
        public static readonly string ExternalSourceCallError = "Calling the external source failed.";
        public static readonly string CurrencyServiceCallError = "Calling the currency service failed.";
        public static readonly string InvalidRequest = "The request is invalid.";
        public static readonly string InvalidConfiguration = "The configuration is incomplete/invalid.";
        public static readonly string Unknown = "Unknown error.";

        public static string GetDefaultMessage(ExceptionType exceptionType)
        {
            return exceptionType switch
            {
                ExceptionType.InvoiceNotFound => InvoiceNotFound,
                ExceptionType.ExternalSourceNotSupported => ExternalSourceNotSupported,
                ExceptionType.ExternalSourceCallError => ExternalSourceCallError,
                ExceptionType.CurrencyServiceCallError => CurrencyServiceCallError,
                ExceptionType.InvalidRequest => InvalidRequest,
                ExceptionType.InvalidConfiguration => InvalidConfiguration,
                ExceptionType.Unknown => Unknown,
                _ => Unknown,
            };
        }
    }
}
