using AutoMapper;
using Compago.Domain;
using Compago.Service;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Service.Settings;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Compago.Test.Service
{
    public class ServiceTest
    {
        protected static readonly IMapper _mapper = MapperHelper.DefineMapper();

        protected static readonly IOptions<ExternalSourceSettings.GSuite> _gSuiteDefaultOptions = OptionsHelper.DefineGSuiteSettingOptions();
        protected static readonly IOptions<ExternalSourceSettings.MicrosoftAzure> _microsoftAzureDefaultOptions = OptionsHelper.DefineMicrosoftAzureSettingOptions();
        protected static readonly IOptions<CurrencyServiceSettings.EX> _exDefaultOptions = OptionsHelper.DefineEXSettingOptions();

        protected static readonly ILogger<DelegateService> _delegateServiceLogger = Substitute.For<ILogger<DelegateService>>();
        protected static readonly ILogger<GSuiteService> _gSuiteServiceLogger = Substitute.For<ILogger<GSuiteService>>();
        protected static readonly ILogger<MicrosoftAzureService> _microsoftAzureServiceLogger = Substitute.For<ILogger<MicrosoftAzureService>>();
        protected static readonly ILogger<CurrencyService> _currencyServiceLogger = Substitute.For<ILogger<CurrencyService>>();

        protected static readonly IDelegateService _delegateService = Substitute.For<IDelegateService>();
        protected static readonly IGSuiteService _gSuiteService = Substitute.For<IGSuiteService>();
        protected static readonly IMicrosoftAzureService _microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
        protected static readonly ICurrencyService _currencyService = Substitute.For<ICurrencyService>();
        protected static readonly IInvoiceTagService _invoiceTagService = Substitute.For<IInvoiceTagService>();

        protected static readonly int _cacheUserId = 999;

        protected static ICacheService GetCacheService()
        {
            var cacheService = new CacheService();
            var userSecurityCredentials = UserSecurityCredentialsHelper.New(id: _cacheUserId);
            cacheService.Set<UserSecurityCredentialsDTO>(userSecurityCredentials);
            return cacheService;
        }
    }
}
