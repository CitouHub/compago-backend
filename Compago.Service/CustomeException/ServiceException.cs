namespace Compago.Service.CustomeException
{
    public class ServiceException(
        ExceptionType exceptionType,
        Exception? innerException = null,
        string? details = null) : Exception(
            $"{ExceptionMessage.GetDefaultMessage(exceptionType)} {details}",
            innerException)
    {
        public readonly ExceptionType ExceptionType = exceptionType;
    }
}
