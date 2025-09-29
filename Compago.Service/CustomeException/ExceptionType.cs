namespace Compago.Service.CustomeException
{
    public enum ExceptionType
    {
        ItemNotFound,
        ItemAlreadyExist,
        InvalidRequest,
        ExternalSourceNotSupported,
        ExternalSourceCallError,
        CurrencyServiceCallError,
        InvalidConfiguration,
        Unknown
    }
}
