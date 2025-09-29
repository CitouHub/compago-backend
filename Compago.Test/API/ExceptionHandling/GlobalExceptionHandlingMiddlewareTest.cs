using Compago.API.ExceptionHandling;
using Compago.Common;
using Compago.Service.CustomeException;
using Compago.Test.Helper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Net.Http.Json;

namespace Compago.Test.API.ExceptionHandling
{
    public class GlobalExceptionHandlingMiddlewareTest
    {
        [Theory]
        [InlineData(ExceptionType.ItemNotFound, HttpStatusCode.NotFound)]
        [InlineData(ExceptionType.InvalidRequest, HttpStatusCode.BadRequest)]
        [InlineData(ExceptionType.ExternalSourceNotSupported, HttpStatusCode.Forbidden)]
        [InlineData(ExceptionType.ExternalSourceCallError, HttpStatusCode.InternalServerError)]
        [InlineData(ExceptionType.CurrencyServiceCallError, HttpStatusCode.InternalServerError)]
        [InlineData(ExceptionType.InvalidConfiguration, HttpStatusCode.InternalServerError)]
        [InlineData(ExceptionType.Unknown, HttpStatusCode.InternalServerError)]
        public async Task ServiceException(ExceptionType exceptionType, HttpStatusCode expectedHttpStatusCode)
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.MockDelegateService.GetBillingAsync(
                Arg.Any<SupportedExternalSource>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string?>())
                .Throws(new ServiceException(exceptionType));

            // Act
            var client = app.CreateClient();
            var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{SupportedExternalSource.GSuite}/{DateTime.Now:yyyy-MM-dd}/{DateTime.Now:yyyy-MM-dd}");
            var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

            // Assert
            Assert.Equal((int)expectedHttpStatusCode, result?.Status);
            Assert.Equal(expectedHttpStatusCode, response.StatusCode);
        }

        [Fact]
        public async Task ServiceException_WithDetails()
        {
            // Arrange
            var app = new CompagoAPIMock();
            var details = "ExceptionDetails";
            app.MockDelegateService.GetBillingAsync(
                Arg.Any<SupportedExternalSource>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string?>())
                .Throws(new ServiceException(ExceptionType.InvalidRequest, details: details));

            // Act
            var client = app.CreateClient();
            var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{SupportedExternalSource.GSuite}/{DateTime.Now:yyyy-MM-dd}/{DateTime.Now:yyyy-MM-dd}");
            var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

            // Assert
            Assert.Contains(details, result!.Details.First());
        }

        [Fact]
        public async Task ServiceException_InnerException()
        {
            // Arrange
            var app = new CompagoAPIMock();
            var exceptionMessage = "InnerTestException";
            app.MockDelegateService.GetBillingAsync(
                Arg.Any<SupportedExternalSource>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string?>())
                .Throws(new ServiceException(ExceptionType.InvalidRequest, innerException: new Exception(exceptionMessage)));

            // Act
            var client = app.CreateClient();
            var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{SupportedExternalSource.GSuite}/{DateTime.Now:yyyy-MM-dd}/{DateTime.Now:yyyy-MM-dd}");
            var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

            // Assert
            Assert.Equal(exceptionMessage, result!.Details[1]);
        }

        [Fact]
        public async Task UnknownException()
        {
            // Arrange
            var app = new CompagoAPIMock();
            app.MockDelegateService.GetBillingAsync(
                Arg.Any<SupportedExternalSource>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string?>())
                .Throws(new Exception());

            // Act
            var client = app.CreateClient();
            var response = await client.GetAsync($"{Constants.API_VERSION}/billing/{SupportedExternalSource.GSuite}/{DateTime.Now:yyyy-MM-dd}/{DateTime.Now:yyyy-MM-dd}");
            var result = await response.Content.ReadFromJsonAsync<ErrorDTO>();

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, result?.Status);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
