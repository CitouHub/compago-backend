using AutoMapper;
using Compago.Mapping;
using Microsoft.Extensions.Logging;

namespace Compago.Test.Helper
{
    public static class MapperHelper
    {
        public static IMapper DefineMapper()
        {
            var logger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            });
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfiles([
                    new GSuiteProfile(),
                    new MicrosoftAzureProfile()
                ]);
            }, logger);

            return mapperConfig.CreateMapper();
        }
    }
}
