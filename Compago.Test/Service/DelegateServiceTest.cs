using Compago.Common;
using Compago.Service;
using Compago.Service.CustomeException;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Test.Helper.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Compago.Test.Service
{
    public class DelegateServiceTest : ServiceTest
    {
        public class GetBilling
        {
            public class GSuite
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService);
                    var exceptionType = ExceptionType.Unknown;
                    gSuiteService.GetBillingAsync(
                        Arg.Any<DateTime>(),
                        Arg.Any<DateTime>())
                        .Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        delegateService.GetBillingAsync(SupportedExternalSource.GSuite, DateTime.UtcNow, DateTime.UtcNow));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await gSuiteService.Received(1).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    gSuiteService.GetBillingAsync(from, to)
                        .Returns(billing);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(billing, result);

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData(" ")]
                public async Task Success_WithInvoices_NoCurrencyConvert(string? currency)
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    gSuiteService.GetBillingAsync(from, to)
                        .Returns(billing);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, from, to, currency);

                    // Assert
                    Assert.NotNull(result);

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Fact]
                public async Task Success_WithInvoices_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, currencyService);

                    var fromCurrency = "from";
                    var toCurrency = "to";
                    var billing = BillingHelper.New(currency: fromCurrency);

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02));
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04));
                    billing.Invoices = [invoice1, invoice2];

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    gSuiteService.GetBillingAsync(fromDate, toDate)
                        .Returns(billing);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(billing.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(billing.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(toCurrency.ToUpper(), result.Currency);
                    Assert.Equal(fromCurrency.ToUpper(), result.OrigialCurrency);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);

                    await gSuiteService.Received(1).GetBillingAsync(fromDate, toDate);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                }
            }

            public class MicrosoftAzure
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService);
                    var exceptionType = ExceptionType.Unknown;
                    microsoftAzureService.GetBillingAsync(
                        Arg.Any<DateTime>(),
                        Arg.Any<DateTime>())
                        .Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, DateTime.UtcNow, DateTime.UtcNow));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    microsoftAzureService.GetBillingAsync(from, to)
                        .Returns(billing);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(billing, result);

                    await _gSuiteService.Received(0).GetBillingAsync(from, to);
                    await microsoftAzureService.Received(1).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData(" ")]
                public async Task Success_WithInvoices_NoCurrencyConvert(string? currency)
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    microsoftAzureService.GetBillingAsync(from, to)
                        .Returns(billing);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, from, to, currency);

                    // Assert
                    Assert.NotNull(result);

                    await _gSuiteService.Received(0).GetBillingAsync(from, to);
                    await microsoftAzureService.Received(1).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                }

                [Fact]
                public async Task Success_WithInvoices_CurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, currencyService);

                    var fromCurrency = "from";
                    var toCurrency = "to";
                    var billing = BillingHelper.New(currency: fromCurrency);

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02));
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04));
                    billing.Invoices = [invoice1, invoice2];

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    microsoftAzureService.GetBillingAsync(fromDate, toDate)
                        .Returns(billing);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(billing.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(billing.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(toCurrency.ToUpper(), result.Currency);
                    Assert.Equal(fromCurrency.ToUpper(), result.OrigialCurrency);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);

                    await _gSuiteService.Received(0).GetBillingAsync(fromDate, toDate);
                    await microsoftAzureService.Received(1).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                }
            }
        }
    }
}
