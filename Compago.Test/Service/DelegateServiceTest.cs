using Compago.Common;
using Compago.Domain;
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
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService, _invoiceTagService);
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
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_NoDataReturned()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService, _invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    gSuiteService.GetBillingAsync(from, to)
                        .Returns((BillingDTO?)null);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.Null(result);

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService, _invoiceTagService);
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
                    Assert.Equal(SupportedExternalSource.GSuite, billing.Source);

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    gSuiteService.GetBillingAsync(from, to)
                        .Returns(billing);

                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns((List<InvoiceTagDTO>?)null);
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.GSuite, billing.Source);
                    Assert.Empty(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Empty(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoice_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, _currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    gSuiteService.GetBillingAsync(from, to)
                        .Returns(billing);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.GSuite, billing.Source);
                    Assert.Single(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Single(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(1).GetBillingAsync(from, to);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, currencyService, invoiceTagService);

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
                    Assert.Equal(SupportedExternalSource.GSuite, billing.Source);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice2.Id).ToList());

                    await gSuiteService.Received(1).GetBillingAsync(fromDate, toDate);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, gSuiteService, _microsoftAzureService, currencyService, invoiceTagService);

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

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.GSuite, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(toCurrency.ToUpper(), result.Currency);
                    Assert.Equal(fromCurrency.ToUpper(), result.OrigialCurrency);
                    Assert.Equal(SupportedExternalSource.GSuite, billing.Source);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice2.Id).ToList());

                    await gSuiteService.Received(1).GetBillingAsync(fromDate, toDate);
                    await _microsoftAzureService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }
            }

            public class MicrosoftAzure
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService, _invoiceTagService);
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
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_NoDataReturned()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService, _invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    microsoftAzureService.GetBillingAsync(from, to)
                        .Returns((BillingDTO?)null);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.Null(result);

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(from, to);
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService, _invoiceTagService);
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
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, billing.Source);

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(from, to);
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await _invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    microsoftAzureService.GetBillingAsync(from, to)
                        .Returns(billing);

                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns((List<InvoiceTagDTO>?)null);
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, billing.Source);
                    Assert.Empty(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Empty(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(from, to);
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoice_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, _currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var billing = BillingHelper.New();
                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    billing.Invoices = [invoice1, invoice2];
                    microsoftAzureService.GetBillingAsync(from, to)
                        .Returns(billing);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, billing.Source);
                    Assert.Single(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Single(result.Invoices.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(from, to);
                    await _currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

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
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, billing.Source);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice2.Id).ToList());

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(fromDate, toDate);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var delegateService = new DelegateService(_delegateServiceLogger, _gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

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

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await delegateService.GetBillingAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(toCurrency.ToUpper(), result.Currency);
                    Assert.Equal(fromCurrency.ToUpper(), result.OrigialCurrency);
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, billing.Source);
                    Assert.Equal(billing.Invoices.Count, result.Invoices.Count);
                    Assert.Equal(price1 * exchangeRate1, result.Invoices.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.Invoices.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Invoices.Where(_ => _.Id == invoice2.Id).ToList());

                    await _gSuiteService.Received(0).GetBillingAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetBillingAsync(fromDate, toDate);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrency.ToUpper(), toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }
            }
        }
    }
}
