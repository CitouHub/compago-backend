using Compago.Service.CustomeException;
using System.Net;
using System.Text.Json;

namespace Compago.API.ExceptionHandling
{
    public class GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var error = GetErroResponse(exception);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)error.Item1;

            return context.Response.WriteAsync(JsonSerializer.Serialize(error.Item2));
        }

        public static (HttpStatusCode, ErrorDTO) GetErroResponse(Exception exception)
        {
            HttpStatusCode status;
            string typeUrl;
            string title;

            if (exception is ServiceException serviceException)
            {
                switch (serviceException.ExceptionType)
                {
                    case ExceptionType.InvalidRequest:
                        status = HttpStatusCode.BadRequest;
                        title = "Request invalid";
                        typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                        break;
                    case ExceptionType.ExternalSourceNotSupported:
                        status = HttpStatusCode.Forbidden;
                        title = "Forbidden request";
                        typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
                        break;
                    case ExceptionType.InvoiceNotFound:
                        status = HttpStatusCode.NotFound;
                        title = "Entity not found";
                        typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
                        break;
                    case ExceptionType.ExternalSourceCallError:
                    case ExceptionType.InvalidConfiguration:
                    case ExceptionType.Unknown:
                        status = HttpStatusCode.InternalServerError;
                        title = "Unable to process request";
                        typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                        break;
                    default:
                        status = HttpStatusCode.InternalServerError;
                        title = "Unable to process request";
                        typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                        break;
                }
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                title = "Unable to process request";
                typeUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            }

            return (status, new ErrorDTO
            {
                Type = typeUrl,
                Title = title,
                Status = (int)status,
                Errors = new List<string>() {
                    exception.Message,
                    exception.InnerException?.Message ?? "",
                    exception.InnerException?.InnerException?.Message ?? "",
                    exception.StackTrace ?? ""
                }
            });
        }
    }
}
