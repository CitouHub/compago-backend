using AutoMapper;
using Compago.Service;
using Compago.Service.Config;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Test.Helper;
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

        protected static readonly IDelegateService _delegateService = Substitute.For<IDelegateService>();
        protected static readonly IGSuiteService _gSuiteService = Substitute.For<IGSuiteService>();
        protected static readonly IMicrosoftAzureService _microsoftAzureService = Substitute.For<IMicrosoftAzureService>();
        protected static readonly ICurrencyService _currencyService = Substitute.For<ICurrencyService>();
    }
}
