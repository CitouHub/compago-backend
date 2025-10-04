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
    public class ExternalSourceServiceTest : ServiceTest
    {
        public class GetInvoices
        {
            public class GSuite
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var exceptionType = ExceptionType.Unknown;
                    gSuiteService.GetInvoicesAsync(
                        Arg.Any<DateTime>(),
                        Arg.Any<DateTime>())
                        .Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, DateTime.UtcNow, DateTime.UtcNow));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await gSuiteService.Received(1).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_NoDataReturned()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    gSuiteService.GetInvoicesAsync(from, to)
                        .Returns((List<InvoiceDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.Null(result);

                    await gSuiteService.Received(1).GetInvoicesAsync(from, to);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var invoice = InvoiceHelper.New();
                    gSuiteService.GetInvoicesAsync(from, to)
                        .Returns([invoice]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Single(result);
                    Assert.Empty(result.First().InvoiceTags);
                    Assert.Equal(SupportedExternalSource.GSuite, result.First().Source);

                    await gSuiteService.Received(1).GetInvoicesAsync(from, to);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    gSuiteService.GetInvoicesAsync(from, to)
                        .Returns([invoice1, invoice2]);

                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns((List<InvoiceTagDTO>?)null);
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.GSuite));
                    Assert.Empty(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Empty(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(1).GetInvoicesAsync(from, to);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoice_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    gSuiteService.GetInvoicesAsync(from, to)
                        .Returns([invoice1, invoice2]);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.GSuite));
                    Assert.Single(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Single(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(1).GetInvoicesAsync(from, to);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02), currency: fromCurrency);
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04), currency: fromCurrency);

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };
                    gSuiteService.GetInvoicesAsync(fromDate, toDate)
                        .Returns(invoices);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(invoice1.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(invoice2.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(invoices.Count, result.Count);
                    Assert.Equal(price1 * exchangeRate1, result.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Where(_ => _.Id == invoice2.Id).ToList());

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.GSuite));
                    Assert.True(result.All(_ => _.Currency == toCurrencyUpper));
                    Assert.True(result.All(_ => _.OriginalCurrency == fromCurrencyUpper));

                    await gSuiteService.Received(1).GetInvoicesAsync(fromDate, toDate);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02), currency: fromCurrency);
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04), currency: fromCurrency);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    gSuiteService.GetInvoicesAsync(fromDate, toDate)
                        .Returns(invoices);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(invoice1.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(invoice2.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.GSuite, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(invoices.Count, result.Count);
                    Assert.Equal(price1 * exchangeRate1, result.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Where(_ => _.Id == invoice2.Id).ToList());

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.GSuite));
                    Assert.True(result.All(_ => _.Currency == toCurrencyUpper));
                    Assert.True(result.All(_ => _.OriginalCurrency == fromCurrencyUpper));

                    await gSuiteService.Received(1).GetInvoicesAsync(fromDate, toDate);
                    await microsoftAzureService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice2.Date);
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
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var exceptionType = ExceptionType.Unknown;
                    microsoftAzureService.GetInvoicesAsync(
                        Arg.Any<DateTime>(),
                        Arg.Any<DateTime>())
                        .Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, DateTime.UtcNow, DateTime.UtcNow));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_NoDataReturned()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    microsoftAzureService.GetInvoicesAsync(from, to)
                        .Returns((List<InvoiceDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.Null(result);

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(from, to);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutInvoices_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    microsoftAzureService.GetInvoicesAsync(from, to)
                        .Returns([]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Empty(result);

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(from, to);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };
                    microsoftAzureService.GetInvoicesAsync(from, to)
                        .Returns(invoices);

                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns((List<InvoiceTagDTO>?)null);
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.MicrosoftAzure));
                    Assert.Empty(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Empty(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(from, to);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoice_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var from = new DateTime(2025, 01, 02);
                    var to = new DateTime(2025, 03, 04);

                    var invoice1 = InvoiceHelper.New("id1", 100);
                    var invoice2 = InvoiceHelper.New("id2", 200);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };
                    microsoftAzureService.GetInvoicesAsync(from, to)
                        .Returns(invoices);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, from, to);

                    // Assert
                    Assert.NotNull(result);
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.MicrosoftAzure));
                    Assert.Single(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());
                    Assert.Single(result.FirstOrDefault(_ => _.Id == invoice1.Id)!.InvoiceTags.ToList());

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(from, to);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02), currency: fromCurrency);
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04), currency: fromCurrency);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    microsoftAzureService.GetInvoicesAsync(fromDate, toDate)
                        .Returns(invoices);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(invoice1.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(invoice2.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(invoices.Count, result.Count);
                    Assert.Equal(price1 * exchangeRate1, result.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Where(_ => _.Id == invoice2.Id).ToList());

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.MicrosoftAzure));
                    Assert.True(result.All(_ => _.Currency == toCurrencyUpper));
                    Assert.True(result.All(_ => _.OriginalCurrency == fromCurrencyUpper));

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(fromDate, toDate);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }

                [Fact]
                public async Task Success_WithInvoices_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price1 = 100;
                    var price2 = 200;
                    var invoice1 = InvoiceHelper.New("id1", price1, new DateTime(2025, 01, 02), currency: fromCurrency);
                    var invoice2 = InvoiceHelper.New("id2", price2, new DateTime(2025, 03, 04), currency: fromCurrency);
                    var invoices = new List<InvoiceDTO>() { invoice1, invoice2 };

                    var fromDate = new DateTime(2025, 01, 02);
                    var toDate = new DateTime(2025, 12, 31);
                    microsoftAzureService.GetInvoicesAsync(fromDate, toDate)
                        .Returns(invoices);

                    var exchangeRate1 = 1.23;
                    var exchangeRate2 = 1.24;
                    currencyService.GetExchangeRateAsync(invoice1.Currency, toCurrency, invoice1.Date)
                        .Returns(exchangeRate1);
                    currencyService.GetExchangeRateAsync(invoice2.Currency, toCurrency, invoice2.Date)
                        .Returns(exchangeRate2);

                    var invoiceTag1 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice1.Id)
                        .Returns([invoiceTag1]);
                    var invoiceTag2 = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice2.Id)
                        .Returns([invoiceTag2]);

                    // Act
                    var result = await externalSourceService.GetInvoicesAsync(SupportedExternalSource.MicrosoftAzure, fromDate, toDate, toCurrency);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(invoices.Count, result.Count);
                    Assert.Equal(price1 * exchangeRate1, result.First(_ => _.Id == invoice1.Id).Price);
                    Assert.Equal(price2 * exchangeRate2, result.First(_ => _.Id == invoice2.Id).Price);
                    Assert.Single(result.Where(_ => _.Id == invoice1.Id).ToList());
                    Assert.Single(result.Where(_ => _.Id == invoice2.Id).ToList());

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.True(result.All(_ => _.Source == SupportedExternalSource.MicrosoftAzure));
                    Assert.True(result.All(_ => _.Currency == toCurrencyUpper));
                    Assert.True(result.All(_ => _.OriginalCurrency == fromCurrencyUpper));

                    await gSuiteService.Received(0).GetInvoicesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
                    await microsoftAzureService.Received(1).GetInvoicesAsync(fromDate, toDate);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice1.Date);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice2.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice1.Id);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice2.Id);
                }
            }
        }

        public class GetInvoice
        {
            public class GSuite
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var exceptionType = ExceptionType.Unknown;
                    gSuiteService.GetInvoiceAsync(Arg.Any<string>()).Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, "1"));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await gSuiteService.Received(1).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task InvoiceNotFound()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);
                    var invoiceId = "1";
                    gSuiteService.GetInvoiceAsync(invoiceId).Returns((InvoiceDTO?)null);

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, invoiceId));
                    Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                    Assert.Contains(SupportedExternalSource.GSuite.ToString(), exception.Message);
                    Assert.Contains(invoiceId, exception.Message);

                    await gSuiteService.Received(1).GetInvoiceAsync(invoiceId);
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var invoiceId = "1";
                    var invoice = InvoiceHelper.New(invoiceId, 100);
                    gSuiteService.GetInvoiceAsync(invoiceId).Returns(invoice);
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id).Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, invoiceId);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.GSuite, result.Source);
                    Assert.Empty(result.InvoiceTags);

                    await gSuiteService.Received(1).GetInvoiceAsync(invoiceId);
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var invoiceId = "1";
                    var invoice = InvoiceHelper.New(invoiceId, 100);
                    gSuiteService.GetInvoiceAsync(invoiceId).Returns(invoice);

                    var invoiceTag = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id).Returns([invoiceTag]);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, invoiceId);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.GSuite, result.Source);
                    Assert.Single(result.InvoiceTags);

                    await gSuiteService.Received(1).GetInvoiceAsync(invoiceId);
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price = 100;
                    var invoice = InvoiceHelper.New("id1", price, new DateTime(2025, 01, 02), currency: fromCurrency);
                    gSuiteService.GetInvoiceAsync(invoice.Id)
                        .Returns(invoice);

                    var exchangeRate = 1.23;
                    currencyService.GetExchangeRateAsync(invoice.Currency, toCurrency, invoice.Date)
                        .Returns(exchangeRate);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, invoice.Id, toCurrency);

                    // Assert
                    Assert.NotNull(result); ;
                    Assert.Equal(price * exchangeRate, invoice.Price);

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.Equal(SupportedExternalSource.GSuite, invoice.Source);
                    Assert.Equal(toCurrencyUpper, invoice.Currency);
                    Assert.Equal(fromCurrencyUpper, invoice.OriginalCurrency);

                    await gSuiteService.Received(1).GetInvoiceAsync(invoice.Id);
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var price = 100;
                    var invoice = InvoiceHelper.New("id1", price, new DateTime(2025, 01, 02), currency: fromCurrency);
                    gSuiteService.GetInvoiceAsync(invoice.Id).Returns(invoice);

                    var exchangeRate = 1.23;
                    currencyService.GetExchangeRateAsync(invoice.Currency, toCurrency, invoice.Date)
                        .Returns(exchangeRate);

                    var invoiceTag = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id)
                        .Returns([invoiceTag]);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.GSuite, invoice.Id, toCurrency);

                    // Assert
                    Assert.NotNull(result); ;
                    Assert.Equal(price * exchangeRate, invoice.Price);

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.Equal(SupportedExternalSource.GSuite, invoice.Source);
                    Assert.Equal(toCurrencyUpper, invoice.Currency);
                    Assert.Equal(fromCurrencyUpper, invoice.OriginalCurrency);

                    await gSuiteService.Received(1).GetInvoiceAsync(invoice.Id);
                    await microsoftAzureService.Received(0).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }
            }

            public class MicrosoftAzure
            {
                [Fact]
                public async Task ServiceException()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var exceptionType = ExceptionType.Unknown;
                    microsoftAzureService.GetInvoiceAsync(Arg.Any<long>()).Throws(new ServiceException(exceptionType));

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, "1".ToString()));
                    Assert.Equal(exceptionType, exception.ExceptionType);

                    await gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(Arg.Any<long>());
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task InvoiceNotFound()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var invoiceId = 1;
                    microsoftAzureService.GetInvoiceAsync(invoiceId).Returns((InvoiceDTO?)null);

                    // Act / Assert
                    var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                        externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, invoiceId.ToString()));
                    Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                    Assert.Contains(SupportedExternalSource.MicrosoftAzure.ToString(), exception.Message);
                    Assert.Contains(invoiceId.ToString(), exception.Message);

                    await _gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(invoiceId);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(0).GetInvoiceTagsAsync(Arg.Any<string>());
                }

                [Fact]
                public async Task Success_WithoutTags_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var invoiceId = 1;
                    var invoice = InvoiceHelper.New(invoiceId.ToString(), 100);
                    microsoftAzureService.GetInvoiceAsync(invoiceId).Returns(invoice);
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id).Returns((List<InvoiceTagDTO>?)null);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, invoiceId.ToString());

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, result.Source);
                    Assert.Empty(result.InvoiceTags);

                    await gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(invoiceId);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithTagss_NoCurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var invoiceId = 1;
                    var invoice = InvoiceHelper.New(invoiceId.ToString(), 100);
                    microsoftAzureService.GetInvoiceAsync(invoiceId).Returns(invoice);

                    var invoiceTag = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id).Returns([invoiceTag]);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, invoiceId.ToString());

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, result.Source);
                    Assert.Single(result.InvoiceTags);

                    await gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(invoiceId);
                    await currencyService.Received(0).GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithoutTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var invoiceId = 1;
                    var price = 100;
                    var invoice = InvoiceHelper.New(invoiceId.ToString(), price, new DateTime(2025, 01, 02), currency: fromCurrency);
                    microsoftAzureService.GetInvoiceAsync(invoiceId).Returns(invoice);

                    var exchangeRate = 1.23;
                    currencyService.GetExchangeRateAsync(invoice.Currency, toCurrency, invoice.Date)
                        .Returns(exchangeRate);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, invoice.Id, toCurrency);

                    // Assert
                    Assert.NotNull(result); ;
                    Assert.Equal(price * exchangeRate, invoice.Price);

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, invoice.Source);
                    Assert.Equal(toCurrencyUpper, invoice.Currency);
                    Assert.Equal(fromCurrencyUpper, invoice.OriginalCurrency);

                    await gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(invoiceId);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }

                [Fact]
                public async Task Success_WithTags_CurrencyConvert()
                {
                    // Arrange
                    var gSuiteService = Substitute.For<IGSuiteService>();
                    var microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
                    var currencyService = Substitute.For<ICurrencyService>();
                    var invoiceTagService = Substitute.For<IInvoiceTagService>();
                    var externalSourceService = new ExternalSourceService(_externalSourceServiceLogger, gSuiteService, microsoftAzureService, currencyService, invoiceTagService);

                    var fromCurrency = "from";
                    var toCurrency = "to";

                    var invoiceId = 1;
                    var price = 100;
                    var invoice = InvoiceHelper.New(invoiceId.ToString(), price, new DateTime(2025, 01, 02), currency: fromCurrency);
                    microsoftAzureService.GetInvoiceAsync(invoiceId).Returns(invoice);

                    var exchangeRate = 1.23;
                    currencyService.GetExchangeRateAsync(invoice.Currency, toCurrency, invoice.Date)
                        .Returns(exchangeRate);

                    var invoiceTag = InvoiceTagHelper.New();
                    invoiceTagService.GetInvoiceTagsAsync(invoice.Id)
                        .Returns([invoiceTag]);

                    // Act
                    var result = await externalSourceService.GetInvoiceAsync(SupportedExternalSource.MicrosoftAzure, invoice.Id, toCurrency);

                    // Assert
                    Assert.NotNull(result); ;
                    Assert.Equal(price * exchangeRate, invoice.Price);

                    var toCurrencyUpper = toCurrency.ToUpper();
                    var fromCurrencyUpper = fromCurrency.ToUpper();
                    Assert.Equal(SupportedExternalSource.MicrosoftAzure, invoice.Source);
                    Assert.Equal(toCurrencyUpper, invoice.Currency);
                    Assert.Equal(fromCurrencyUpper, invoice.OriginalCurrency);

                    await gSuiteService.Received(0).GetInvoiceAsync(Arg.Any<string>());
                    await microsoftAzureService.Received(1).GetInvoiceAsync(invoiceId);
                    await currencyService.Received(1).GetExchangeRateAsync(fromCurrencyUpper, toCurrency, invoice.Date);
                    await invoiceTagService.Received(1).GetInvoiceTagsAsync(invoice.Id);
                }
            }
        }
    }
}
